using BusinessLogic.Servicios.Inventario;
using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Repositorios.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.ViewModels.Inventario;
using System.Globalization;

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
        public async Task<IActionResult> Index(
    string? q,
    int? estadoId,
    int? tipoId,
    int pageNumber = 1,
    int pageSize = 10,
    string sortBy = "Codigo",
    string sortDir = "asc")
        {
            ViewData["Title"] = "Gestión de Activos de Equipos";

            // Seguridad básica
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            // Sanitizar sorting
            sortBy = (sortBy ?? "Codigo").Trim();
            sortDir = (sortDir ?? "asc").Trim().ToLower();
            if (sortDir != "asc" && sortDir != "desc") sortDir = "asc";

            // Whitelist columnas válidas
            var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    { "Codigo", "Nombre", "Tipo", "Estado" };
            if (!allowed.Contains(sortBy)) sortBy = "Codigo";

            // Cargar combos (se mantiene como lo tenías para no mover capas hoy)
            var estados = (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre);
            var tipos = (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre);

            var filtros = new ActivoInventarioFiltroDto
            {
                Texto = q,
                IdEstadoActivo = estadoId,
                IdTipoActivo = tipoId,
                Page = pageNumber,
                PageSize = pageSize,

                // NUEVO
                SortBy = sortBy,
                SortDir = sortDir
            };

            var result = await _inventario.ListarPaginadoAsync(filtros);

            var vm = new InventarioIndexViewModel
            {
                Items = result.Items?.ToList() ?? new List<ActivoInventarioListItemDto>(),

                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords,

                Q = q,
                EstadoId = estadoId,
                TipoId = tipoId,

                // NUEVO
                SortBy = sortBy,
                SortDir = sortDir,

                Estados = new SelectList(estados, "IdEstadoActivo", "Nombre", estadoId),
                Tipos = new SelectList(tipos, "IdTipoActivo", "Nombre", tipoId)
            };

            return View(vm);
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
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo);
                return View(model);
            }

            var dto = new ActivoInventarioCreateDto
            {
                NumeroActivo = model.NumeroActivo,
                NombreMaquina = model.NombreMaquina,
                SerieServicio = model.SerieServicio,
                IdTipoActivo = model.IdTipoActivo,
                IdEstadoActivo = model.IdEstadoActivo
            };

            var (ok, error) = await _inventario.CrearAsync(dto);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo crear el activo.");
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Editar Activo";

            var detalle = await _inventario.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            await CargarCatalogosAsync(detalle.IdTipoActivo, detalle.IdEstadoActivo);

            var vm = new CrearActivoViewModel
            {
                NumeroActivo = detalle.NumeroActivo,
                NombreMaquina = detalle.NombreMaquina ?? "",
                SerieServicio = detalle.SerieServicio ?? "",
                IdTipoActivo = detalle.IdTipoActivo,
                IdEstadoActivo = detalle.IdEstadoActivo
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CrearActivoViewModel model)
        {
            ViewData["Title"] = "Editar Activo";

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo);
                return View(model);
            }

            var dto = new ActivoInventarioEditDto
            {
                // NumeroActivo NO se cambia (regla)
                NombreMaquina = model.NombreMaquina,
                SerieServicio = model.SerieServicio,
                IdTipoActivo = model.IdTipoActivo,
                IdEstadoActivo = model.IdEstadoActivo
            };

            var (ok, error) = await _inventario.ActualizarAsync(id, dto);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo actualizar el activo.");
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Detalle del Activo";

            var detalle = await _inventario.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            return View(detalle);
        }

        [HttpGet]
        public async Task<IActionResult> DetailModal(int id)
        {
            var detalle = await _inventario.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            return PartialView("_DetailModal", detalle);
        }
        private async Task CargarCatalogosAsync(int? tipoIdSeleccionado = null, int? estadoIdSeleccionado = null)
        {
            ViewBag.Estados = new SelectList(
                (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre),
                "IdEstadoActivo", "Nombre", estadoIdSeleccionado);

            ViewBag.Tipos = new SelectList(
                (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre),
                "IdTipoActivo", "Nombre", tipoIdSeleccionado);
        }
    }
}