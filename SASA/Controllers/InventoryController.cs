using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        // Listado + búsqueda + filtros (texto, estado, tipo)
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
        // Crear Activo (Sprint 1 - HU #47/#49 se implementan luego)
        // =========================
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Registrar Activo";
            return View();
        }

        // =========================
        // Detalle del Activo (se implementa luego en Sprint 1)
        // =========================
        [HttpGet]
        public IActionResult Detail(int id)
        {
            ViewData["Title"] = "Detalle del Activo";
            return View();
        }

        // =========================
        // Editar Activo (Sprint 1 - HU #53 se implementa luego)
        // =========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar Activo";
            return View();
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

        // GET: /Inventory/Equipment/Create
        [HttpGet]
        public IActionResult CreateEquipment()
        {
            return View();
        }

        // GET: /Inventory/Maintenance/Create
        [HttpGet]
        public IActionResult CreateMaintenance()
        {
            return View();
        }

        // GET: /Inventory/Reports/General
        [HttpGet]
        public IActionResult GeneralReport()
        {
            return View();
        }

        // POST: /Inventory/TicketAssociation
        [HttpPost]
        public IActionResult AssociateTicket()
        {
            return View("AssociationResult");
        }

        [HttpGet]
        public IActionResult MaintenanceReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult StatusReport()
        {
            return View();
        }
    }
}
