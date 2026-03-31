using BusinessLogic.Servicios.Inventario;
using BusinessLogic.Servicios.Tiquetes;
using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Modelos.Entidades.Integracion;
using DataAccess.Repositorios.Integracion;
using DataAccess.Repositorios.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.Filters;
using SASA.ViewModels.Inventario;
using System;
using System.Security.Claims;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize(Roles = "Administrador")]
    public class InventoryController : Controller
    {
        private readonly IInventarioService _inventario;
        private readonly ICatalogosInventarioRepository _catRepo;
        private readonly ITiqueteService _tiqueteService;
        private readonly IIntegracionHistorialRepository _histRepo;

        public InventoryController(
            IInventarioService inventario,
            ICatalogosInventarioRepository catRepo,
            ITiqueteService tiqueteService,
            IIntegracionHistorialRepository histRepo)
        {
            _inventario = inventario;
            _catRepo = catRepo;
            _tiqueteService = tiqueteService;
            _histRepo = histRepo;
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
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
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
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
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

            await CargarCatalogosAsync(detalle.IdTipoActivo, detalle.IdEstadoActivo, detalle.IdTipoLicencia);

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

            var detalleActual = await _inventario.ObtenerDetalleAsync(id);
            if (detalleActual == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            bool sinCambios =
                (detalleActual.NumeroActivo?.Trim() ?? string.Empty) == (model.NumeroActivo?.Trim() ?? string.Empty) &&
                (detalleActual.NombreMaquina?.Trim() ?? string.Empty) == (model.NombreMaquina?.Trim() ?? string.Empty) &&
                detalleActual.IdTipoActivo == model.IdTipoActivo &&
                detalleActual.IdEstadoActivo == model.IdEstadoActivo &&
                (detalleActual.Marca?.Trim() ?? string.Empty) == (model.Marca?.Trim() ?? string.Empty) &&
                (detalleActual.Modelo?.Trim() ?? string.Empty) == (model.Modelo?.Trim() ?? string.Empty) &&
                (detalleActual.SerieServicio?.Trim() ?? string.Empty) == (model.SerieServicio?.Trim() ?? string.Empty) &&
                (detalleActual.DireccionMAC?.Trim() ?? string.Empty) == (model.DireccionMAC?.Trim() ?? string.Empty) &&
                (detalleActual.SistemaOperativo?.Trim() ?? string.Empty) == (model.SistemaOperativo?.Trim() ?? string.Empty) &&
                detalleActual.IdTipoLicencia == model.IdTipoLicencia &&
                (detalleActual.ClaveLicencia?.Trim() ?? string.Empty) == (model.ClaveLicencia?.Trim() ?? string.Empty);

            if (sinCambios)
            {
                ModelState.AddModelError(string.Empty, "No se detectaron cambios para guardar.");
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            var dto = new ActivoInventarioEditDto
            {
                NumeroActivo = model.NumeroActivo ?? string.Empty,
                NombreMaquina = model.NombreMaquina ?? string.Empty,
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
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
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

            await CargarCatalogosAsync(detalle.IdTipoActivo, detalle.IdEstadoActivo, detalle.IdTipoLicencia);

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
            var detalleActual = await _inventario.ObtenerDetalleAsync(id);
            if (detalleActual == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return PartialView("_EditModal", model);
            }

            bool sinCambios =
                (detalleActual.NumeroActivo?.Trim() ?? string.Empty) == (model.NumeroActivo?.Trim() ?? string.Empty) &&
                (detalleActual.NombreMaquina?.Trim() ?? string.Empty) == (model.NombreMaquina?.Trim() ?? string.Empty) &&
                detalleActual.IdTipoActivo == model.IdTipoActivo &&
                detalleActual.IdEstadoActivo == model.IdEstadoActivo &&
                (detalleActual.Marca?.Trim() ?? string.Empty) == (model.Marca?.Trim() ?? string.Empty) &&
                (detalleActual.Modelo?.Trim() ?? string.Empty) == (model.Modelo?.Trim() ?? string.Empty) &&
                (detalleActual.SerieServicio?.Trim() ?? string.Empty) == (model.SerieServicio?.Trim() ?? string.Empty) &&
                (detalleActual.DireccionMAC?.Trim() ?? string.Empty) == (model.DireccionMAC?.Trim() ?? string.Empty) &&
                (detalleActual.SistemaOperativo?.Trim() ?? string.Empty) == (model.SistemaOperativo?.Trim() ?? string.Empty) &&
                detalleActual.IdTipoLicencia == model.IdTipoLicencia &&
                (detalleActual.ClaveLicencia?.Trim() ?? string.Empty) == (model.ClaveLicencia?.Trim() ?? string.Empty);

            if (sinCambios)
            {
                ModelState.AddModelError(string.Empty, "No se detectaron cambios para guardar.");
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return PartialView("_EditModal", model);
            }

            var dto = new ActivoInventarioEditDto
            {
                NumeroActivo = model.NumeroActivo ?? string.Empty,
                NombreMaquina = model.NombreMaquina ?? string.Empty,
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
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return PartialView("_EditModal", model);
            }

            return Json(new { success = true });
        }

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

        [HttpGet]
        public async Task<IActionResult> Maintenance(int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Historial de Mantenimiento";

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var allItems = (await _inventario.ObtenerHistorialMantenimientoAsync()).ToList();
            var totalRecords = allItems.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (totalPages < 1) totalPages = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            var items = allItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new MantenimientoIndexViewModel
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateMaintenance()
        {
            ViewData["Title"] = "Registrar Mantenimiento";

            var activos = await _inventario.ObtenerActivosParaAsociacionAsync();

            var vm = new CrearMantenimientoViewModel
            {
                FechaMantenimiento = DateTime.Today,
                Activos = new SelectList(
                    activos.Select(a => new
                    {
                        a.IdActivo,
                        Texto = $"{a.NumeroActivo} - {a.NombreMaquina}"
                    }),
                    "IdActivo",
                    "Texto")
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaintenance(CrearMantenimientoViewModel model)
        {
            ViewData["Title"] = "Registrar Mantenimiento";

            if (!ModelState.IsValid)
            {
                var activosError = await _inventario.ObtenerActivosParaAsociacionAsync();
                model.Activos = new SelectList(
                    activosError.Select(a => new
                    {
                        a.IdActivo,
                        Texto = $"{a.NumeroActivo} - {a.NombreMaquina}"
                    }),
                    "IdActivo",
                    "Texto",
                    model.IdActivo);

                return View(model);
            }

            var dto = new CrearMantenimientoActivoDto
            {
                IdActivo = model.IdActivo,
                FechaMantenimiento = model.FechaMantenimiento,
                TipoMantenimiento = model.TipoMantenimiento,
                Estado = model.Estado,
                Descripcion = model.Descripcion
            };

            var (ok, error) = await _inventario.RegistrarMantenimientoAsync(dto);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo registrar el mantenimiento.");

                var activosError = await _inventario.ObtenerActivosParaAsociacionAsync();
                model.Activos = new SelectList(
                    activosError.Select(a => new
                    {
                        a.IdActivo,
                        Texto = $"{a.NumeroActivo} - {a.NombreMaquina}"
                    }),
                    "IdActivo",
                    "Texto",
                    model.IdActivo);

                return View(model);
            }

            TempData["Success"] = "El mantenimiento fue registrado correctamente.";
            return RedirectToAction(nameof(Maintenance));
        }

        [HttpGet]
        public async Task<IActionResult> EditMaintenanceModal(int id)
        {
            var data = await _inventario.ObtenerMantenimientoPorIdAsync(id);
            if (data == null) return NotFound();

            var activos = await _inventario.ObtenerActivosParaAsociacionAsync();

            var vm = new CrearMantenimientoViewModel
            {
                IdMantenimiento = data.IdMantenimiento,
                IdActivo = data.IdActivo,
                FechaMantenimiento = data.FechaMantenimiento,
                TipoMantenimiento = data.TipoMantenimiento,
                Estado = data.Estado,
                Descripcion = data.Descripcion,
                Activos = new SelectList(
                    activos.Select(a => new
                    {
                        a.IdActivo,
                        Texto = $"{a.NumeroActivo} - {a.NombreMaquina}"
                    }),
                    "IdActivo",
                    "Texto",
                    data.IdActivo)
            };

            return PartialView("_EditMaintenanceModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaintenanceModal(int id, CrearMantenimientoViewModel model)
        {
            var activos = await _inventario.ObtenerActivosParaAsociacionAsync();
            model.Activos = new SelectList(
                activos.Select(a => new
                {
                    a.IdActivo,
                    Texto = $"{a.NumeroActivo} - {a.NombreMaquina}"
                }),
                "IdActivo",
                "Texto",
                model.IdActivo);

            if (!ModelState.IsValid)
                return PartialView("_EditMaintenanceModal", model);

            var dto = new CrearMantenimientoActivoDto
            {
                IdActivo = model.IdActivo,
                FechaMantenimiento = model.FechaMantenimiento,
                TipoMantenimiento = model.TipoMantenimiento,
                Estado = model.Estado,
                Descripcion = model.Descripcion
            };

            var (ok, error) = await _inventario.ActualizarMantenimientoAsync(id, dto);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "No se pudo actualizar el mantenimiento.");
                return PartialView("_EditMaintenanceModal", model);
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult Reports()
        {
            ViewData["Title"] = "Reportes de Inventario";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GeneralReport(int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Reporte General de Inventario";

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var allItems = (await _inventario.ObtenerActivosReporteGeneralAsync()).ToList();
            var totalRecords = allItems.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (totalPages < 1) totalPages = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            var items = allItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new ReporteGeneralInventarioViewModel
            {
                TotalActivos = allItems.Count,
                ActivosActivos = allItems.Count(x =>
                    string.Equals(x.EstadoActivoNombre, "Activo", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.EstadoActivoNombre, "Operativo", StringComparison.OrdinalIgnoreCase)),
                EnMantenimiento = allItems.Count(x =>
                    string.Equals(x.EstadoActivoNombre, "Mantenimiento", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.EstadoActivoNombre, "En Mantenimiento", StringComparison.OrdinalIgnoreCase)),
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> StatusReport()
        {
            ViewData["Title"] = "Reporte por Estado";

            var resumen = await _inventario.ObtenerResumenPorEstadoAsync();

            var vm = new ReporteEstadoInventarioViewModel
            {
                Estados = resumen
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> MaintenanceReport(int pageNumber = 1, int pageSize = 10)
        {
            ViewData["Title"] = "Reporte de Mantenimiento";

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 5) pageSize = 5;
            if (pageSize > 50) pageSize = 50;

            var allItems = (await _inventario.ObtenerHistorialMantenimientoAsync()).ToList();
            var totalRecords = allItems.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            if (totalPages < 1) totalPages = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            var items = allItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new MantenimientoIndexViewModel
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalRecords = totalRecords
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string? q, int? estadoId, int? tipoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nombreArchivo = $"Inventario_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            try
            {
                var archivo = await _inventario.ExportarInventarioExcelAsync(q, tipoId, estadoId);

                var hist = new IntegracionHistorial
                {
                    TipoProceso = "Exportacion",
                    Modulo = "Inventario",
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

                TempData["Success"] = "Exportación generada correctamente.";

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
                    Modulo = "Inventario",
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

                TempData["Error"] = $"No se pudo exportar el inventario: {ex.Message}";
                return RedirectToAction(nameof(Index), new { q, estadoId, tipoId });
            }
        }

        private async Task CargarCatalogosAsync(
            int? tipoIdSeleccionado = null,
            int? estadoIdSeleccionado = null,
            int? licenciaIdSeleccionada = null)
        {
            ViewBag.Estados = new SelectList(
                (await _catRepo.ObtenerEstadosAsync()).OrderBy(e => e.Nombre),
                "IdEstadoActivo", "Nombre", estadoIdSeleccionado);

            ViewBag.Tipos = new SelectList(
                (await _catRepo.ObtenerTiposAsync()).OrderBy(t => t.Nombre),
                "IdTipoActivo", "Nombre", tipoIdSeleccionado);

            ViewBag.Licencias = new SelectList(
                (await _catRepo.ObtenerLicenciasAsync()).OrderBy(l => l.Nombre),
                "IdTipoLicencia", "Nombre", licenciaIdSeleccionada);
        }
    }
}