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
                _logger.LogInformation(
                    "Inicio de carga de integración. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, Archivo: {Archivo}, Tamano: {Tamano}",
                    userId,
                    tipoCarga,
                    archivo?.FileName,
                    archivo?.Length);

                if (archivo == null || archivo.Length == 0)
                {
                    _logger.LogWarning(
                        "Carga rechazada: archivo nulo o vacío. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}",
                        userId,
                        tipoCarga);

                    ModelState.AddModelError("", "Debe seleccionar un archivo.");
                    return View();
                }

                if (archivo.Length > MaxFileSizeBytes)
                {
                    _logger.LogWarning(
                        "Carga rechazada: archivo excede tamaño máximo. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, Archivo: {Archivo}, Tamano: {Tamano}",
                        userId,
                        tipoCarga,
                        archivo.FileName,
                        archivo.Length);

                    ModelState.AddModelError("", $"El archivo supera el tamaño máximo permitido ({MaxFileSizeBytes / (1024 * 1024)}MB).");
                    return View();
                }

                if (tipoCarga != "Equipos" && tipoCarga != "Telefonos")
                {
                    _logger.LogWarning(
                        "Carga rechazada: tipo de carga inválido. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, Archivo: {Archivo}",
                        userId,
                        tipoCarga,
                        archivo?.FileName);

                    ModelState.AddModelError("", "Debe seleccionar un tipo de integración válido.");
                    return View();
                }

                var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                if (ext != ".xlsx")
                {
                    _logger.LogWarning(
                        "Carga rechazada: extensión inválida. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, Archivo: {Archivo}, Extension: {Extension}",
                        userId,
                        tipoCarga,
                        archivo.FileName,
                        ext);

                    ModelState.AddModelError("", "Formato inválido. Debe ser .xlsx");
                    return View();
                }

                var carpeta = Path.Combine(_env.WebRootPath, "uploads", "integracion");
                Directory.CreateDirectory(carpeta);

                var originalName = Path.GetFileName(archivo.FileName);
                var safeName = string.Join("_", originalName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
                                    .Replace(" ", "_");

                var nombreSeguro = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{safeName}";
                var rutaFisica = Path.Combine(carpeta, nombreSeguro);

                _logger.LogInformation(
                    "Guardando archivo de integración. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, ArchivoOriginal: {ArchivoOriginal}, RutaFisica: {RutaFisica}",
                    userId,
                    tipoCarga,
                    originalName,
                    rutaFisica);

                await using (var stream = System.IO.File.Create(rutaFisica))
                {
                    await archivo.CopyToAsync(stream);
                }

                var historialId = await _integracion.RegistrarCargaAsync(
                    nombreArchivoOriginal: originalName,
                    rutaArchivoRelativa: $"/uploads/integracion/{nombreSeguro}",
                    usuarioId: userId,
                    tipoCarga: tipoCarga
                );

                _logger.LogInformation(
                    "Carga registrada correctamente. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, HistorialId: {HistorialId}, Archivo: {Archivo}",
                    userId,
                    tipoCarga,
                    historialId,
                    originalName);

                return RedirectToAction(nameof(Validation), new { historialId });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inesperado al subir archivo de integración. Usuario: {UsuarioId}, TipoCarga: {TipoCarga}, Archivo: {Archivo}, Tamano: {Tamano}",
                    userId,
                    tipoCarga,
                    archivo?.FileName,
                    archivo?.Length);

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

                if (historial.Modulo == "InventarioTelefonos")
                {
                    _logger.LogInformation(
                        "Validando archivo de teléfonos. HistorialId: {HistorialId}, Archivo: {Archivo}",
                        historialId,
                        historial.NombreArchivo);

                    var dtoTelefonos = await _integracion.ValidarTelefonosDesdeExcelAsync(historialId, _env.WebRootPath);

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

                var dtoEquipos = await _integracion.ValidarInventarioDesdeExcelAsync(historialId, _env.WebRootPath);

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

            var resultado = await _integracion.ReprocesarArchivoAsync(historialId, _env.WebRootPath, userId);

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