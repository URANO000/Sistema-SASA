using BusinessLogic.Servicios.InventarioTelefono;
using DataAccess.Modelos.DTOs.InventarioTelefono;
using DataAccess.Modelos.Entidades.Integracion;
using DataAccess.Repositorios.Integracion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.InventarioTelefono;
using System.Security.Claims;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize(Roles = "Administrador")]
    public class InventoryPhoneController : Controller
    {
        private readonly IActivoTelefonoService _service;
        private readonly IIntegracionHistorialRepository _histRepo;

        public InventoryPhoneController(
            IActivoTelefonoService service,
            IIntegracionHistorialRepository histRepo)
        {
            _service = service;
            _histRepo = histRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int pageNumber = 1, int pageSize = 10, string sortBy = "Nombre", string sortDir = "asc")
        {
            ViewData["Title"] = "Gestión de Activos Teléfono";

            var filtros = new ActivoTelefonoFiltroDto
            {
                Texto = q,
                Page = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDir = sortDir
            };

            var result = await _service.ListarPaginadoAsync(filtros);

            var vm = new TelefonoIndexViewModel
            {
                Items = result.Items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = (int)Math.Ceiling(result.TotalRecords / (double)pageSize),
                Q = q,
                SortBy = sortBy,
                SortDir = sortDir
            };

            if (vm.TotalPages < 1) vm.TotalPages = 1;

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Registrar Teléfono";
            return View(new CrearTelefonoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearTelefonoViewModel model)
        {
            ViewData["Title"] = "Registrar Teléfono";

            if (!ModelState.IsValid)
                return View(model);

            var (ok, error) = await _service.CrearAsync(model.ToCreateDto());

            if (!ok)
            {
                ModelState.AddModelError(nameof(model.IMEI), error ?? "No se pudo registrar el teléfono.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DetailModal(int id)
        {
            var data = await _service.ObtenerDetalleAsync(id);

            if (data == null)
                return NotFound();

            return PartialView("_DetailModal", data);
        }

        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var data = await _service.ObtenerDetalleAsync(id);

            if (data == null)
                return NotFound();

            var vm = new CrearTelefonoViewModel
            {
                IdActivoTelefono = data.IdActivoTelefono,
                NombreColaborador = data.NombreColaborador,
                Departamento = data.Departamento,
                Operador = data.Operador,
                NumeroCelular = data.NumeroCelular,
                CorreoSistemasAnaliticos = data.CorreoSistemasAnaliticos,
                Modelo = data.Modelo,
                IMEI = data.IMEI,
                Cargador = data.Cargador,
                Auriculares = data.Auriculares
            };

            return PartialView("_EditModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, CrearTelefonoViewModel model)
        {
            if (id != model.IdActivoTelefono)
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar correctamente el teléfono a editar.");
                return PartialView("_EditModal", model);
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_EditModal", model);
            }

            var (ok, error) = await _service.ActualizarAsync(id, model.ToEditDto());

            if (!ok)
            {
                ModelState.AddModelError(nameof(model.IMEI), error ?? "No se pudo actualizar el teléfono.");
                return PartialView("_EditModal", model);
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? q, string sortBy = "Nombre", string sortDir = "asc")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nombreArchivo = $"Telefonos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            try
            {
                var filtros = new ActivoTelefonoFiltroDto
                {
                    Texto = q,
                    SortBy = sortBy,
                    SortDir = sortDir,
                    Page = 1,
                    PageSize = 100000
                };

                var archivo = await _service.ExportarExcelAsync(filtros);

                var hist = new IntegracionHistorial
                {
                    TipoProceso = "Exportacion",
                    Modulo = "InventarioTelefonos",
                    NombreArchivo = nombreArchivo,
                    RutaArchivo = string.Empty,
                    Fecha = DateTime.UtcNow,
                    Estado = "Exportado",
                    DetalleError = null,
                    UsuarioEjecutorId = userId,
                    TotalFilas = 0,
                    FilasValidas = 0,
                    FilasConError = 0
                };

                await _histRepo.CrearAsync(hist);
                await _histRepo.GuardarAsync();

                TempData["Success"] = "Exportación de teléfonos generada correctamente.";

                return File(
                    archivo,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo
                );
            }
            catch (Exception ex)
            {
                var hist = new IntegracionHistorial
                {
                    TipoProceso = "Exportacion",
                    Modulo = "InventarioTelefonos",
                    NombreArchivo = nombreArchivo,
                    RutaArchivo = string.Empty,
                    Fecha = DateTime.UtcNow,
                    Estado = "Fallido",
                    DetalleError = ex.Message,
                    UsuarioEjecutorId = userId,
                    TotalFilas = 0,
                    FilasValidas = 0,
                    FilasConError = 0
                };

                await _histRepo.CrearAsync(hist);
                await _histRepo.GuardarAsync();

                TempData["Error"] = $"No se pudo exportar el inventario de teléfonos: {ex.Message}";
                return RedirectToAction(nameof(Index), new { q, sortBy, sortDir });
            }
        }
    }
}