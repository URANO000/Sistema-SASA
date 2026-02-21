using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using SASA.Models;
using SASA.Filters;
using Microsoft.AspNetCore.Authorization;


namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataAccess.ApplicationDbContext _db;
        private readonly BusinessLogic.Servicios.Tiquetes.ITiqueteService _tiqueteService;

        public HomeController(ILogger<HomeController> logger, DataAccess.ApplicationDbContext db, BusinessLogic.Servicios.Tiquetes.ITiqueteService tiqueteService)
        {
            _logger = logger;
            _db = db;
            _tiqueteService = tiqueteService;
        }

        [HttpGet]
        public IActionResult GetDashboardJson()
        {
            var role = User?.Identity != null && User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                : null;

            var counts = _db.Tiquetes.AsNoTracking()
                .GroupBy(t => t.IdEstatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var vm = new SASA.ViewModels.Home.DashboardViewModel
            {
                Abiertos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Abierto)?.Count ?? 0,
                EnProgreso = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProgreso)?.Count ?? 0,
                Resueltos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)?.Count ?? 0,
                Cancelados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)?.Count ?? 0,
                Cerrados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cerrado)?.Count ?? 0,
                Rol = role
            };
            var prioridades = _db.Prioridades.AsNoTracking().OrderBy(p => p.IdPrioridad).ToList();
            var prioridadCounts = _db.Tiquetes.AsNoTracking()
                .GroupBy(t => t.IdPrioridad)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToList();

            vm.PriorityLabels = prioridades.Select(p => p.NombrePrioridad).ToArray();
            vm.PriorityCounts = prioridades.Select(p => prioridadCounts.FirstOrDefault(pc => pc.Id == p.IdPrioridad)?.Count ?? 0).ToArray();

            var days = 7;
            var today = DateTime.Today;
            var from = today.AddDays(-(days - 1));

            var created = _db.Tiquetes.AsNoTracking()
                .Where(t => t.CreatedAt >= from)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var closed = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cerrado)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var cancelled = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var labels = new List<string>();
            var creadosList = new List<int>();
            var cerradosList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                cerradosList.Add(closed.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels.ToArray();
            vm.TrendCreados = creadosList.ToArray();
            vm.TrendCerrados = cerradosList.ToArray();
            vm.TrendCancelados = canceladosList.ToArray();

            return Json(vm);
        }

        public IActionResult Index()
        {
            var role = User?.Identity != null && User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                : null;

            var counts = _db.Tiquetes.AsNoTracking()
                .GroupBy(t => t.IdEstatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var vm = new SASA.ViewModels.Home.DashboardViewModel
            {
                Abiertos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Abierto)?.Count ?? 0,
                EnProgreso = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProgreso)?.Count ?? 0,
                Resueltos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)?.Count ?? 0,
                Cancelados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)?.Count ?? 0,
                Cerrados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cerrado)?.Count ?? 0,
                Rol = role
            };
            var prioridades = _db.Prioridades.AsNoTracking().OrderBy(p => p.IdPrioridad).ToList();
            var prioridadCounts = _db.Tiquetes.AsNoTracking()
                .GroupBy(t => t.IdPrioridad)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToList();

            vm.PriorityLabels = prioridades.Select(p => p.NombrePrioridad).ToArray();
            vm.PriorityCounts = prioridades.Select(p => prioridadCounts.FirstOrDefault(pc => pc.Id == p.IdPrioridad)?.Count ?? 0).ToArray();
            var days = 7;
            var today = DateTime.Today;
            var from = today.AddDays(-(days - 1));

            var created = _db.Tiquetes.AsNoTracking()
                .Where(t => t.CreatedAt >= from)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var closed = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cerrado)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var cancelled = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var labels = new List<string>();
            var creadosList = new List<int>();
            var cerradosList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                cerradosList.Add(closed.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels.ToArray();
            vm.TrendCreados = creadosList.ToArray();
            vm.TrendCerrados = cerradosList.ToArray();
            vm.TrendCancelados = canceladosList.ToArray();

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
