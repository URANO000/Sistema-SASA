using BusinessLogic.Servicios.Inventario;
using BusinessLogic.Servicios.Tiquetes;
using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Repositorios.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using SASA.Filters;
using SASA.ViewModels.Inventario;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize(Roles = "Administrador")]
    public class InventoryController : Controller
    {
        private readonly IInventarioService _inventario;
        private readonly ICatalogosInventarioRepository _catRepo;
        private readonly ITiqueteService _tiqueteService;

        public InventoryController(
            IInventarioService inventario,
            ICatalogosInventarioRepository catRepo,
            ITiqueteService tiqueteService)
        {
            _inventario = inventario;
            _catRepo = catRepo;
            _tiqueteService = tiqueteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? q, int? estadoId, int? tipoId, int pageNumber = 1, int pageSize = 10, string sortBy = "Codigo", string sortDir = "desc")
        {
            ViewData["Title"] = "Gestión de Activos de Equipos";

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var estados = (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre);
            var tipos = (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre);

            var filtros = new ActivoInventarioFiltroDto
            {
                Texto = q,
                IdEstadoActivo = estadoId,
                IdTipoActivo = tipoId,
                Page = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDir = sortDir
            };

            var result = await _inventario.ListarPaginadoAsync(filtros);

            var vm = new InventarioIndexViewModel
            {
                Items = result.Items?.ToList() ?? new List<ActivoTelefonoInventarioListItemDto>(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords,

                Q = q,
                EstadoId = estadoId,
                TipoId = tipoId,

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
                NumeroActivo = detalle.NumeroActivo ?? "",
                NombreMaquina = detalle.NombreMaquina ?? "",
                SerieServicio = detalle.SerieServicio,

                IdTipoActivo = detalle.IdTipoActivo,
                IdEstadoActivo = detalle.IdEstadoActivo,

                Marca = detalle.Marca,
                Modelo = detalle.Modelo,
                DireccionMAC = detalle.DireccionMAC,
                SistemaOperativo = detalle.SistemaOperativo,

                IdTipoLicencia = detalle.IdTipoLicencia,
                ClaveLicencia = detalle.ClaveLicencia
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
                NumeroActivo = model.NumeroActivo,
                NombreMaquina = model.NombreMaquina,
                SerieServicio = model.SerieServicio,
                IdTipoActivo = model.IdTipoActivo,
                IdEstadoActivo = model.IdEstadoActivo,

                Marca = model.Marca,
                Modelo = model.Modelo,
                DireccionMAC = model.DireccionMAC,
                SistemaOperativo = model.SistemaOperativo,

                IdTipoLicencia = model.IdTipoLicencia,
                ClaveLicencia = model.ClaveLicencia
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
        public async Task<IActionResult> DetailModal(int id)
        {
            var detalle = await _inventario.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            return PartialView("_DetailModal", detalle);
        }

        [HttpGet]
        public async Task<IActionResult> EditModal(int id)
        {
            var detalle = await _inventario.ObtenerDetalleAsync(id);
            if (detalle == null) return NotFound();

            await CargarCatalogosAsync(detalle.IdTipoActivo, detalle.IdEstadoActivo);

            var vm = new CrearActivoViewModel
            {
                NumeroActivo = detalle.NumeroActivo ?? "",
                NombreMaquina = detalle.NombreMaquina ?? "",

                IdTipoActivo = detalle.IdTipoActivo,
                IdEstadoActivo = detalle.IdEstadoActivo,

                Marca = detalle.Marca,
                Modelo = detalle.Modelo,
                SerieServicio = detalle.SerieServicio,
                DireccionMAC = detalle.DireccionMAC,
                SistemaOperativo = detalle.SistemaOperativo,

                IdTipoLicencia = detalle.IdTipoLicencia,
                ClaveLicencia = detalle.ClaveLicencia
            };

            return PartialView("_EditModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditModal(int id, CrearActivoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo);
                return PartialView("_EditModal", model);
            }

            var dto = new ActivoInventarioEditDto
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

            var (ok, error) = await _inventario.ActualizarAsync(id, dto);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo actualizar el activo.");
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo);
                return PartialView("_EditModal", model);
            }

            return Json(new { success = true });
        }

        // ==========================
        // ASOCIAR ACTIVO CON TIQUETE
        // ==========================

        [HttpGet]
        public async Task<IActionResult> TicketAssociation()
        {
            ViewData["Title"] = "Asociación de Activos con Tiquetes";

            var vm = new AsociacionActivoTiqueteViewModel();

            var activos = await _inventario.ObtenerActivosParaAsociacionAsync();
            var tiquetes = await _tiqueteService.ObtenerTiquetesReporteAsync();

            vm.Activos = activos
                .Select(a => new SelectListItem
                {
                    Value = a.IdActivo.ToString(),
                    Text = $"{a.NumeroActivo} - {a.NombreMaquina}"
                }).ToList();

            vm.Tiquetes = tiquetes
                .Select(t => new SelectListItem
                {
                    Value = t.IdTiquete.ToString(),
                    Text = $"TCK-{t.IdTiquete} - {t.Asunto}"
                }).ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TicketAssociation(AsociacionActivoTiqueteViewModel model)
        {
            ViewData["Title"] = "Asociación de Activos con Tiquetes";

            var activos = await _inventario.ObtenerActivosParaAsociacionAsync();
            var tiquetes = await _tiqueteService.ObtenerTiquetesReporteAsync();

            model.Activos = activos.Select(a => new SelectListItem
            {
                Value = a.IdActivo.ToString(),
                Text = $"{a.NumeroActivo} - {a.NombreMaquina}"
            }).ToList();

            model.Tiquetes = tiquetes.Select(t => new SelectListItem
            {
                Value = t.IdTiquete.ToString(),
                Text = $"TCK-{t.IdTiquete} - {t.Asunto}"
            }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var dto = new ActivoTiqueteAsociacionDto
            {
                IdActivo = model.IdActivo!.Value,
                IdTiquete = model.IdTiquete!.Value
            };

            var (ok, error) = await _inventario.AsociarActivoConTiqueteAsync(dto);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo realizar la asociación.");
                return View(model);
            }

            TempData["Success"] = "El activo fue asociado correctamente al tiquete.";
            return RedirectToAction(nameof(TicketAssociation));
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