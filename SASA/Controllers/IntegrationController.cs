using BusinessLogic.Servicios.Integracion;
using Microsoft.AspNetCore.Mvc;
using SASA.ViewModels.Integracion;
using System.Security.Claims;

namespace SASA.Controllers
{
    public class IntegrationController : Controller
    {
        private readonly IIntegracionService _integracion;
        private readonly IWebHostEnvironment _env;

        // 10MB (ajustable)
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
        public async Task<IActionResult> Upload(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo.");
                return View();
            }

            // ✅ NUEVO: tamaño máximo
            if (archivo.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError("", $"El archivo supera el tamaño máximo permitido ({MaxFileSizeBytes / (1024 * 1024)}MB).");
                return View();
            }

            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (ext != ".xlsx")
            {
                ModelState.AddModelError("", "Formato inválido. Debe ser .xlsx");
                return View();
            }

            // Guardar físico (MVC puede hacerlo)
            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "integracion");
            Directory.CreateDirectory(carpeta);

            // ✅ NUEVO: nombre seguro
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

            // BD ahora la maneja BusinessLogic
            var historialId = await _integracion.RegistrarCargaAsync(
                nombreArchivoOriginal: originalName,
                rutaArchivoRelativa: $"/uploads/integracion/{nombreSeguro}",
                usuarioId: userId
            );

            return RedirectToAction(nameof(Validation), new { historialId });
        }

        [HttpGet]
        public async Task<IActionResult> Validation(int historialId)
        {
            try
            {
                var dto = await _integracion.ValidarInventarioDesdeExcelAsync(historialId, _env.WebRootPath);

                var vm = new ValidacionImportacionViewModel
                {
                    HistorialId = dto.HistorialId,
                    NombreArchivo = dto.NombreArchivo,
                    TotalFilas = dto.TotalFilas,
                    FilasValidas = dto.FilasValidas,
                    FilasConError = dto.FilasConError,
                    Errores = dto.Errores.Select(e => new FilaValidacion { NumeroFila = e.NumeroFila, Mensaje = e.Mensaje }).ToList(),
                    FilasValidasPreview = dto.FilasValidasPreview.Select(f => new FilaImportacionActivos
                    {
                        NumeroFila = f.NumeroFila,
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

                return View(vm);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Historial no encontrado"))
            {
                TempData["Error"] = "No se encontró el historial solicitado (posible link incorrecto).";
                return RedirectToAction(nameof(History));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmImport(int historialId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _integracion.ConfirmarImportacionInventarioAsync(historialId, _env.WebRootPath, userId);
            return RedirectToAction(nameof(History));
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var data = await _integracion.ObtenerHistorialAsync();
            return View(data);
        }
    }
}