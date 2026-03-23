using BusinessLogic.Servicios.Integracion;
using DataAccess.Modelos.DTOs.Integracion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        private const long MaxFileSizeBytes = 10 * 1024 * 1024;

        public IntegrationController(IIntegracionService integracion, IWebHostEnvironment env)
        {
            _integracion = integracion;
            _env = env;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile archivo, string tipoCarga)
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
                ModelState.AddModelError("", "Debe seleccionar un tipo de integración válido.");
                return View();
            }

            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (ext != ".xlsx")
            {
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

            await using (var stream = System.IO.File.Create(rutaFisica))
            {
                await archivo.CopyToAsync(stream);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var historialId = await _integracion.RegistrarCargaAsync(
                nombreArchivoOriginal: originalName,
                rutaArchivoRelativa: $"/uploads/integracion/{nombreSeguro}",
                usuarioId: userId,
                tipoCarga: tipoCarga
            );

            return RedirectToAction(nameof(Validation), new { historialId });
        }

        [HttpGet]
        public async Task<IActionResult> Validation(int historialId)
        {
            try
            {
                var historial = (await _integracion.ObtenerHistorialAsync())
                    .FirstOrDefault(x => x.Id == historialId);

                if (historial == null)
                {
                    TempData["Error"] = "No se encontró el historial solicitado.";
                    return RedirectToAction(nameof(History));
                }

                if (historial.Modulo == "InventarioTelefonos")
                {
                    var dtoTelefonos = await _integracion.ValidarTelefonosDesdeExcelAsync(historialId, _env.WebRootPath);
                    return View("ValidationPhones", MapToTelefonoViewModel(dtoTelefonos));
                }

                var dtoEquipos = await _integracion.ValidarInventarioDesdeExcelAsync(historialId, _env.WebRootPath);
                return View("Validation", MapToViewModel(dtoEquipos));
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Historial no encontrado"))
            {
                TempData["Error"] = "No se encontró el historial solicitado (posible link incorrecto).";
                return RedirectToAction(nameof(History));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmImport(ValidacionImportacionViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            var data = await _integracion.ObtenerHistorialAsync();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeshabilitarArchivo(int historialId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var resultado = await _integracion.DeshabilitarArchivoAsync(historialId, userId);

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

            var resultado = await _integracion.ReprocesarArchivoAsync(historialId, _env.WebRootPath, userId);

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