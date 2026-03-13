using BusinessLogic.Servicios.InventarioTelefono;
using DataAccess.Modelos.DTOs.InventarioTelefono;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.InventarioTelefono;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize(Roles = "Administrador")]
    public class InventoryPhoneController : Controller
    {
        private readonly IActivoTelefonoService _service;

        public InventoryPhoneController(IActivoTelefonoService service)
        {
            _service = service;
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
    }
}