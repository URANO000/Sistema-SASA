using BusinessLogic.Servicios.Integracion;
using DataAccess.Modelos.DTOs.Integracion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SASA.Filters;
using SASA.ViewModels.Integracion;
using System.Security.Claims;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize(Roles = "Administrador")]
    public class IntegrationController : Controller
    {
        private readonly IIntegracionService _integracion;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<IntegrationController> _logger;

        private const long MaxFileSizeBytes = 10 * 1024 * 1024;

        public IntegrationController(
            IIntegracionService integracion,
            IWebHostEnvironment env,
            ILogger<IntegrationController> logger)
        {
            _integracion = integracion;
            _env = env;
            _logger = logger;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile archivo, string tipoCarga)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                if (archivo == null || archivo.Length == 0)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo.");
                    return View();
                }

                if (archivo.Length > MaxFileSizeBytes)
                {
                    ModelState.AddModelError("", $"El archivo supera el tamaño máximo permitido ({MaxFileSizeBytes / (1024 * 1024)}MB).");
                    return View();
                }

                if (tipoCarga != "Equipos" && tipoCarga != "Telefonos")
                {
                    ModelState.AddModelError("", "Debe seleccionar un tipo válido.");
                    return View();
                }

                var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                if (ext != ".xlsx")
                {
                    ModelState.AddModelError("", "Formato inválido. Debe ser .xlsx");
                    return View();
                }

                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("Usuario no autenticado.");
                }

                //Read file into memory, not wwwroot
                byte[] archivoBytes;
                await using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    archivoBytes = ms.ToArray();
                }

                var historialId = await _integracion.RegistrarCargaAsync(
                    nombreArchivoOriginal: archivo.FileName,
                    archivoBytes: archivoBytes,
                    tipoMime: archivo.ContentType,
                    pesoBytes: archivo.Length,
                    usuarioId: userId,
                    tipoCarga: tipoCarga
                );

                return RedirectToAction(nameof(Validation), new { historialId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo.");
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> Validation(int historialId)
        {
            try
            {
                _logger.LogInformation(
                    "Iniciando validación de integración. HistorialId: {HistorialId}, Usuario: {UsuarioId}",
                    historialId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                var historial = (await _integracion.ObtenerHistorialAsync())
                    .FirstOrDefault(x => x.Id == historialId);

                if (historial == null)
                {
                    _logger.LogWarning(
                        "No se encontró historial para validación. HistorialId: {HistorialId}, Usuario: {UsuarioId}",
                        historialId,
                        User.FindFirstValue(ClaimTypes.NameIdentifier));

                    TempData["Error"] = "No se encontró el historial solicitado.";
                    return RedirectToAction(nameof(History));
                }

                //Validación BLOB
                if (historial.Archivo == null || historial.Archivo.Length == 0)
                {
                    TempData["Error"] = "El archivo asociado no existe o está vacío.";
                    return RedirectToAction(nameof(History));
                }

                if (historial.Modulo == "InventarioTelefonos")
                {
                    _logger.LogInformation(
                        "Validando archivo de teléfonos. HistorialId: {HistorialId}, Archivo: {Archivo}",
                        historialId,
                        historial.NombreArchivo);

                    var dtoTelefonos = await _integracion.ValidarTelefonosDesdeExcelAsync(historialId);

                    _logger.LogInformation(
                        "Validación de teléfonos completada. HistorialId: {HistorialId}, TotalFilas: {TotalFilas}, FilasValidas: {FilasValidas}, FilasConError: {FilasConError}",
                        historialId,
                        dtoTelefonos.TotalFilas,
                        dtoTelefonos.FilasValidas,
                        dtoTelefonos.FilasConError);

                    return View("ValidationPhones", MapToTelefonoViewModel(dtoTelefonos));
                }

                _logger.LogInformation(
                    "Validando archivo de equipos. HistorialId: {HistorialId}, Archivo: {Archivo}",
                    historialId,
                    historial.NombreArchivo);

                var dtoEquipos = await _integracion.ValidarInventarioDesdeExcelAsync(historialId);

                _logger.LogInformation(
                    "Validación de equipos completada. HistorialId: {HistorialId}, TotalFilas: {TotalFilas}, FilasValidas: {FilasValidas}, FilasConError: {FilasConError}",
                    historialId,
                    dtoEquipos.TotalFilas,
                    dtoEquipos.FilasValidas,
                    dtoEquipos.FilasConError);

                return View("Validation", MapToViewModel(dtoEquipos));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Historial no encontrado"))
            {
                _logger.LogWarning(
                    ex,
                    "Historial no encontrado durante validación. HistorialId: {HistorialId}, Usuario: {UsuarioId}",
                    historialId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                TempData["Error"] = "No se encontró el historial solicitado (posible link incorrecto).";
                return RedirectToAction(nameof(History));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inesperado durante validación de integración. HistorialId: {HistorialId}, Usuario: {UsuarioId}",
                    historialId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmImport(ValidacionImportacionViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "Inicio de confirmación de importación de equipos. HistorialId: {HistorialId}, Usuario: {UsuarioId}, TotalFilasEnModelo: {TotalFilas}",
                model.HistorialId,
                userId,
                model.FilasDetectadas?.Count ?? 0);

            var filas = (model.FilasDetectadas ?? new List<FilaImportacionActivos>())
                .Select(f => new FilaImportacionActivosDto
                {
                    NumeroFila = f.NumeroFila,
                    Seleccionado = f.Seleccionado,
                    NumeroActivo = f.NumeroActivo,
                    NombreMaquina = f.NombreMaquina,
                    TipoEquipo = f.TipoEquipo,
                    Marca = f.Marca,
                    Modelo = f.Modelo,
                    SerieServicio = f.SerieServicio,
                    DireccionMAC = f.DireccionMAC,
                    SistemaOperativo = f.SistemaOperativo,
                    TipoLicencia = f.TipoLicencia,
                    ClaveLicencia = f.ClaveLicencia
                })
                .ToList();

            var resultado = await _integracion.ConfirmarImportacionInventarioEditadoAsync(model.HistorialId, filas, userId);

            _logger.LogInformation(
                "Resultado de confirmación de importación de equipos. HistorialId: {HistorialId}, Usuario: {UsuarioId}, Ok: {Ok}, Mensaje: {Mensaje}",
                model.HistorialId,
                userId,
                resultado.Ok,
                resultado.Mensaje);

            if (!resultado.Ok)
            {
                ModelState.AddModelError("", resultado.Mensaje);
                return View("Validation", MapToViewModel(resultado.Resultado));
            }

            TempData["Success"] = resultado.Mensaje;
            return RedirectToAction(nameof(History));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmImportPhones(ValidacionImportacionTelefonoViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "Inicio de confirmación de importación de teléfonos. HistorialId: {HistorialId}, Usuario: {UsuarioId}, TotalFilasEnModelo: {TotalFilas}",
                model.HistorialId,
                userId,
                model.FilasDetectadas?.Count ?? 0);

            var filas = (model.FilasDetectadas ?? new List<FilaImportacionTelefonos>())
                .Select(f => new FilaImportacionTelefonoDto
                {
                    NumeroFila = f.NumeroFila,
                    Seleccionado = f.Seleccionado,
                    NombreColaborador = f.NombreColaborador,
                    Departamento = f.Departamento,
                    Operador = f.Operador,
                    NumeroCelular = f.NumeroCelular,
                    CorreoSistemasAnaliticos = f.CorreoSistemasAnaliticos,
                    Modelo = f.Modelo,
                    IMEI = f.IMEI,
                    Cargador = f.Cargador,
                    Auriculares = f.Auriculares
                })
                .ToList();

            var resultado = await _integracion.ConfirmarImportacionTelefonosAsync(model.HistorialId, filas, userId);

            _logger.LogInformation(
                "Resultado de confirmación de importación de teléfonos. HistorialId: {HistorialId}, Usuario: {UsuarioId}, Ok: {Ok}, Mensaje: {Mensaje}",
                model.HistorialId,
                userId,
                resultado.Ok,
                resultado.Mensaje);

            if (!resultado.Ok)
            {
                ModelState.AddModelError("", resultado.Mensaje);
                return View("ValidationPhones", MapToTelefonoViewModel(resultado.Resultado));
            }

            TempData["Success"] = resultado.Mensaje;
            return RedirectToAction(nameof(History));
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            _logger.LogInformation(
                "Consulta de historial de integración. Usuario: {UsuarioId}",
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            var data = await _integracion.ObtenerHistorialAsync();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeshabilitarArchivo(int historialId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "Solicitud para deshabilitar archivo de integración. HistorialId: {HistorialId}, Usuario: {UsuarioId}",
                historialId,
                userId);

            var resultado = await _integracion.DeshabilitarArchivoAsync(historialId, userId);

            _logger.LogInformation(
                "Resultado de deshabilitar archivo. HistorialId: {HistorialId}, Usuario: {UsuarioId}, Ok: {Ok}, Mensaje: {Mensaje}",
                historialId,
                userId,
                resultado.Ok,
                resultado.Mensaje);

            if (resultado.Ok)
                TempData["Success"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(History));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReprocesarArchivo(int historialId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "Solicitud para reprocesar archivo de integración. HistorialId: {HistorialId}, Usuario: {UsuarioId}",
                historialId,
                userId);

            var resultado = await _integracion.ReprocesarArchivoAsync(historialId, userId);

            _logger.LogInformation(
                "Resultado de reprocesar archivo. HistorialId: {HistorialId}, Usuario: {UsuarioId}, Ok: {Ok}, Mensaje: {Mensaje}",
                historialId,
                userId,
                resultado.Ok,
                resultado.Mensaje);

            if (resultado.Ok)
                TempData["Success"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(History));
        }

        private static ValidacionImportacionViewModel MapToViewModel(ValidacionImportacionDto dto)
        {
            return new ValidacionImportacionViewModel
            {
                HistorialId = dto.HistorialId,
                NombreArchivo = dto.NombreArchivo,
                TotalFilas = dto.TotalFilas,
                FilasValidas = dto.FilasValidas,
                FilasConError = dto.FilasConError,
                TiposEquipoDisponibles = dto.TiposEquipoDisponibles,
                TiposLicenciaDisponibles = dto.TiposLicenciaDisponibles,
                Errores = dto.Errores.Select(e => new FilaValidacion
                {
                    NumeroFila = e.NumeroFila,
                    Mensaje = e.Mensaje
                }).ToList(),
                FilasDetectadas = dto.FilasDetectadas.Select(f => new FilaImportacionActivos
                {
                    NumeroFila = f.NumeroFila,
                    Seleccionado = f.Seleccionado,
                    NumeroActivo = f.NumeroActivo,
                    NombreMaquina = f.NombreMaquina,
                    TipoEquipo = f.TipoEquipo,
                    Marca = f.Marca,
                    Modelo = f.Modelo,
                    SerieServicio = f.SerieServicio,
                    DireccionMAC = f.DireccionMAC,
                    SistemaOperativo = f.SistemaOperativo,
                    TipoLicencia = f.TipoLicencia,
                    ClaveLicencia = f.ClaveLicencia
                }).ToList()
            };
        }

        private static ValidacionImportacionTelefonoViewModel MapToTelefonoViewModel(ValidacionImportacionTelefonoDto dto)
        {
            return new ValidacionImportacionTelefonoViewModel
            {
                HistorialId = dto.HistorialId,
                NombreArchivo = dto.NombreArchivo,
                TotalFilas = dto.TotalFilas,
                FilasValidas = dto.FilasValidas,
                FilasConError = dto.FilasConError,
                Errores = dto.Errores.Select(e => new FilaValidacionTelefono
                {
                    NumeroFila = e.NumeroFila,
                    Mensaje = e.Mensaje
                }).ToList(),
                FilasDetectadas = dto.FilasDetectadas.Select(f => new FilaImportacionTelefonos
                {
                    NumeroFila = f.NumeroFila,
                    Seleccionado = f.Seleccionado,
                    NombreColaborador = f.NombreColaborador,
                    Departamento = f.Departamento,
                    Operador = f.Operador,
                    NumeroCelular = f.NumeroCelular,
                    CorreoSistemasAnaliticos = f.CorreoSistemasAnaliticos,
                    Modelo = f.Modelo,
                    IMEI = f.IMEI,
                    Cargador = f.Cargador,
                    Auriculares = f.Auriculares
                }).ToList()
            };
        }
    }
}