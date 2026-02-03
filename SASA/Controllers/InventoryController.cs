using DataAccess;
using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SASA.ViewModels.Inventario;

namespace SASA.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InventoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // =========================
        // Gestión de Activos (HU #52 - RQFINV-06)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index(string? q, int? estadoId, int? tipoId)
        {
            ViewData["Title"] = "Gestión de Activos";

            // Dropdown Estado
            ViewBag.Estados = new SelectList(
                await _db.EstadoActivoInventario
                    .OrderBy(e => e.Nombre)
                    .ToListAsync(),
                "IdEstadoActivo",
                "Nombre",
                estadoId
            );

            // Dropdown Tipo
            ViewBag.Tipos = new SelectList(
                await _db.TipoActivoInventario
                    .OrderBy(t => t.Nombre)
                    .ToListAsync(),
                "IdTipoActivo",
                "Nombre",
                tipoId
            );

            var query = _db.ActivoInventario
                .Include(a => a.EstadoActivo)
                .Include(a => a.TipoActivo)
                .AsQueryable();

            // Búsqueda por texto (Número / Nombre máquina / Serie)
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(a =>
                    a.NumeroActivo.Contains(q) ||
                    (a.NombreMaquina != null && a.NombreMaquina.Contains(q)) ||
                    (a.SerieServicio != null && a.SerieServicio.Contains(q)));
            }

            // Filtro por Estado
            if (estadoId.HasValue)
                query = query.Where(a => a.IdEstadoActivo == estadoId.Value);

            // Filtro por Tipo
            if (tipoId.HasValue)
                query = query.Where(a => a.IdTipoActivo == tipoId.Value);

            var data = await query
                .OrderByDescending(a => a.FechaCreacion)
                .ToListAsync();

            return View(data);
        }

        // =========================
        // Crear Activo (HU #47 + parte de HU #49: unicidad)
        // =========================
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

            // HU #49 (parte 1): código único
            var existe = await _db.ActivoInventario.AnyAsync(a => a.NumeroActivo == model.NumeroActivo);
            if (existe)
            {
                ModelState.AddModelError(nameof(model.NumeroActivo), "Ya existe un activo con este código.");
                await CargarCatalogosAsync(model.IdTipoActivo, model.IdEstadoActivo, model.IdTipoLicencia);
                return View(model);
            }

            var activo = new ActivoInventario
            {
                NumeroActivo = model.NumeroActivo.Trim(),
                NombreMaquina = model.NombreMaquina.Trim(),

                Marca = model.Marca,
                Modelo = model.Modelo,
                SerieServicio = model.SerieServicio,
                DireccionMAC = model.DireccionMAC,
                SistemaOperativo = model.SistemaOperativo,

                IdTipoActivo = model.IdTipoActivo,
                IdEstadoActivo = model.IdEstadoActivo,
                IdTipoLicencia = model.IdTipoLicencia,
                ClaveLicencia = model.ClaveLicencia,

                FechaCreacion = DateTime.UtcNow
            };

            _db.ActivoInventario.Add(activo);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCatalogosAsync(int? tipoSel = null, int? estadoSel = null, int? licenciaSel = null)
        {
            ViewBag.Tipos = new SelectList(
                await _db.TipoActivoInventario.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync(),
                "IdTipoActivo", "Nombre", tipoSel);

            ViewBag.Estados = new SelectList(
                await _db.EstadoActivoInventario.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
                "IdEstadoActivo", "Nombre", estadoSel);

            ViewBag.Licencias = new SelectList(
                await _db.TipoLicenciaInventario.AsNoTracking().OrderBy(l => l.Nombre).ToListAsync(),
                "IdTipoLicencia", "Nombre", licenciaSel);
        }

        // =========================
        // Detalle del Activo (lo usamos luego para HU #49 con QR)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Detalle del Activo";

            var activo = await _db.ActivoInventario
                .Include(a => a.TipoActivo)
                .Include(a => a.EstadoActivo)
                .FirstOrDefaultAsync(a => a.IdActivo == id);

            if (activo == null)
                return NotFound(); // o RedirectToAction(nameof(Index))

            return View(activo);
        }

        // =========================
        // Editar Activo (HU #53 se hace luego)
        // =========================
        // =========================
        // Editar Activo (HU #53 - Actualizar estado)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Editar Activo";

            var activo = await _db.ActivoInventario
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdActivo == id);

            if (activo == null)
                return NotFound();

            // Catálogos para dropdowns
            ViewBag.Tipos = new SelectList(
                await _db.TipoActivoInventario.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync(),
                "IdTipoActivo", "Nombre", activo.IdTipoActivo);

            ViewBag.Estados = new SelectList(
                await _db.EstadoActivoInventario.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
                "IdEstadoActivo", "Nombre", activo.IdEstadoActivo);

            return View(activo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActivoInventario model)
        {
            if (id != model.IdActivo)
                return BadRequest();

            var activo = await _db.ActivoInventario.FirstOrDefaultAsync(a => a.IdActivo == id);
            if (activo == null)
                return NotFound();

            // ✅ HU #53: actualizar estado
            activo.IdEstadoActivo = model.IdEstadoActivo;

            // Opcional Sprint 1 (si lo quieren permitir)
            activo.IdTipoActivo = model.IdTipoActivo;
            activo.NombreMaquina = model.NombreMaquina;
            activo.Marca = model.Marca;
            activo.Modelo = model.Modelo;
            activo.SerieServicio = model.SerieServicio;
            activo.DireccionMAC = model.DireccionMAC;
            activo.SistemaOperativo = model.SistemaOperativo;
            activo.IdTipoLicencia = model.IdTipoLicencia;
            activo.ClaveLicencia = model.ClaveLicencia;

            activo.FechaActualizacion = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = activo.IdActivo });
        }

        // =========================
        // Equipos (Sprint 2)
        // =========================
        [HttpGet]
        public IActionResult Equipment()
        {
            ViewData["Title"] = "Equipos";
            return View();
        }

        // =========================
        // Historial y Mantenimiento (Sprint 2)
        // =========================
        [HttpGet]
        public IActionResult Maintenance()
        {
            ViewData["Title"] = "Historial de Mantenimiento";
            return View();
        }

        // =========================
        // Asociación Activos - Tiquetes (Sprint 2)
        // =========================
        [HttpGet]
        public IActionResult TicketAssociation()
        {
            ViewData["Title"] = "Asociación de Activos con Tiquetes";
            return View();
        }

        // =========================
        // Reportes (Sprint 3)
        // =========================
        [HttpGet]
        public IActionResult Reports()
        {
            ViewData["Title"] = "Reportes de Inventario";
            return View();
        }

        [HttpGet]
        public IActionResult CreateEquipment() => View();

        [HttpGet]
        public IActionResult CreateMaintenance() => View();

        [HttpGet]
        public IActionResult GeneralReport() => View();

        [HttpPost]
        public IActionResult AssociateTicket() => View("AssociationResult");

        [HttpGet]
        public IActionResult MaintenanceReport() => View();

        [HttpGet]
        public IActionResult StatusReport() => View();
    }
}
