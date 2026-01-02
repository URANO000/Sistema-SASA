using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class InventoryController : Controller
    {
        // Gestión de Activos
        public IActionResult Index()
        {
            ViewData["Title"] = "Gestión de Activos";
            return View();
        }

        // Crear Activo
        public IActionResult Create()
        {
            ViewData["Title"] = "Registrar Activo";
            return View();
        }

        // Detalle del Activo
        public IActionResult Detail(int id)
        {
            ViewData["Title"] = "Detalle del Activo";
            return View();
        }

        // Editar Activo
        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar Activo";
            return View();
        }

        // Equipos
        public IActionResult Equipment()
        {
            ViewData["Title"] = "Equipos";
            return View();
        }

        // Historial y Mantenimiento
        public IActionResult Maintenance()
        {
            ViewData["Title"] = "Historial de Mantenimiento";
            return View();
        }

        // Asociación Activos - Tiquetes
        public IActionResult TicketAssociation()
        {
            ViewData["Title"] = "Asociación de Activos con Tiquetes";
            return View();
        }

        // Reportes
        public IActionResult Reports()
        {
            ViewData["Title"] = "Reportes de Inventario";
            return View();
        }

        // GET: /Inventory/Equipment/Create
        public IActionResult CreateEquipment()
        {
            return View();
        }

        // GET: /Inventory/Maintenance/Create
        public IActionResult CreateMaintenance()
        {
            return View();
        }

        // GET: /Inventory/Reports/General
        public IActionResult GeneralReport()
        {
            return View();
        }

        // POST: /Inventory/TicketAssociation
        public IActionResult AssociateTicket()
        {
            return View("AssociationResult");
        }

        public IActionResult MaintenanceReport()
        {
            return View();
        }

        public IActionResult StatusReport()
        {
            return View();
        }
    }
}
