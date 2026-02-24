using BusinessLogic.Modelos.DTOs.Inventario;
using BusinessLogic.Servicios.Inventario;
using DataAccess.Repositorios.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.ViewModels.Inventario;

namespace SASA.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventarioService _inventario;
        private readonly ICatalogosInventarioRepository _catRepo;

        public InventoryController(IInventarioService inventario, ICatalogosInventarioRepository catRepo)
        {
            _inventario = inventario;
            _catRepo = catRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int? estadoId, int? tipoId, int page = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Gestión de Activos de Equipos";

            // seguridad básica
            if (page < 1) page = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            ViewBag.Estados = new SelectList(
                (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre),
                "IdEstadoActivo", "Nombre", estadoId);

            ViewBag.Tipos = new SelectList(
                (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre),
                "IdTipoActivo", "Nombre", tipoId);

            var filtros = new ActivoInventarioFiltroDto
            {
                Texto = q,
                IdEstadoActivo = estadoId,
                IdTipoActivo = tipoId,
                Page = page,
                PageSize = pageSize
            };

            // 👇 Recomendado: que el service te devuelva (items + total)
            var result = await _inventario.ListarPaginadoAsync(filtros);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = result.TotalPages;

            return View(result.Items);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Registrar Activo";
            await CargarCatalogosAsync();
            return View(new CrearActivoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearActivoViewModel model)
        {
            ViewData["Title"] = "Registrar Activo";

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            var (ok, error) = await _inventario.CrearAsync(model.ToDto());
            if (!ok)
            {
                ModelState.AddModelError(nameof(model.NumeroActivo), error!);
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Detalle del Activo";

            var dto = await _inventario.ObtenerDetalleAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Editar Activo";

            var dto = await _inventario.ObtenerDetalleAsync(id);
            if (dto == null) return NotFound();

            ViewBag.Tipos = new SelectList(
                (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre),
                "IdTipoActivo", "Nombre", dto.IdTipoActivo);

            ViewBag.Estados = new SelectList(
                (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre),
                "IdEstadoActivo", "Nombre", dto.IdEstadoActivo);

            ViewBag.Licencias = new SelectList(
                (await _catRepo.ObtenerLicenciasAsync()).OrderBy(l => l.Nombre),
                "IdTipoLicencia", "Nombre", dto.IdTipoLicencia);

            // Reusando el VM actual para no romper vistas
            var vm = new CrearActivoViewModel
            {
                NumeroActivo = dto.NumeroActivo,
                NombreMaquina = dto.NombreMaquina ?? "",
                IdTipoActivo = dto.IdTipoActivo,
                IdEstadoActivo = dto.IdEstadoActivo,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                SerieServicio = dto.SerieServicio,
                DireccionMAC = dto.DireccionMAC,
                SistemaOperativo = dto.SistemaOperativo,
                IdTipoLicencia = dto.IdTipoLicencia,
                ClaveLicencia = dto.ClaveLicencia
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CrearActivoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            var editDto = new ActivoInventarioEditDto
            {
                NumeroActivo = model.NumeroActivo,
                NombreMaquina = model.NombreMaquina,
                IdTipoActivo = model.IdTipoActivo,
                IdEstadoActivo = model.IdEstadoActivo,
                Marca = model.Marca,
                Modelo = model.Modelo,
                SerieServicio = model.SerieServicio,
                DireccionMAC = model.DireccionMAC,
                SistemaOperativo = model.SistemaOperativo,
                IdTipoLicencia = model.IdTipoLicencia,
                ClaveLicencia = model.ClaveLicencia
            };

            var (ok, error) = await _inventario.ActualizarAsync(id, editDto);
            if (!ok)
            {
                ModelState.AddModelError("", error!);
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        private async Task CargarCatalogosAsync(int? tipoSel = null, int? estadoSel = null, int? licenciaSel = null)
        {
            ViewBag.Tipos = new SelectList(
                (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre),
                "IdTipoActivo", "Nombre", tipoSel);

            ViewBag.Estados = new SelectList(
                (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre),
                "IdEstadoActivo", "Nombre", estadoSel);

            ViewBag.Licencias = new SelectList(
                (await _catRepo.ObtenerLicenciasAsync()).OrderBy(l => l.Nombre),
                "IdTipoLicencia", "Nombre", licenciaSel);
        }

        // stubs (Sprint 2/3)
        [HttpGet] public IActionResult Equipment() { ViewData["Title"] = "Equipos"; return View(); }
        [HttpGet] public IActionResult Maintenance() { ViewData["Title"] = "Historial de Mantenimiento"; return View(); }
        [HttpGet] public IActionResult TicketAssociation() { ViewData["Title"] = "Asociación de Activos con Tiquetes"; return View(); }
        [HttpGet] public IActionResult Reports() { ViewData["Title"] = "Reportes de Inventario"; return View(); }
        [HttpGet] public IActionResult CreateEquipment() => View();
        [HttpGet] public IActionResult CreateMaintenance() => View();
        [HttpGet] public IActionResult GeneralReport() => View();
        [HttpPost] public IActionResult AssociateTicket() => View("AssociationResult");
        [HttpGet] public IActionResult MaintenanceReport() => View();
        [HttpGet] public IActionResult StatusReport() => View();
    }
}