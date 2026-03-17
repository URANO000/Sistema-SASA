using BusinessLogic.Servicios.Inventario;
using BusinessLogic.Servicios.Tiquetes;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SASA.Filters;
using SASA.Models;
using SASA.ViewModels.Home;
using SASA.ViewModels.Tiquete.Extras;
using System.Diagnostics;



namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly ITiqueteService _tiqueteService;
        private readonly IInventarioService _inventarioService;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, ITiqueteService tiqueteService, IInventarioService inventarioService)
        {
            _logger = logger;
            _db = db;
            _tiqueteService = tiqueteService;
            _inventarioService = inventarioService;
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
                Abiertos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Creado)?.Count ?? 0,
                EnProgreso = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProceso)?.Count ?? 0,
                Resueltos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)?.Count ?? 0,
                Cancelados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)?.Count ?? 0,
                EnEsperaDelUsuario = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnEsperaDelUsuario)?.Count ?? 0,
                Rol = role
            };
            var prioridades = _db.Prioridades.AsNoTracking().OrderBy(p => p.IdPrioridad).ToList();
            //var prioridadCounts = _db.Tiquetes.AsNoTracking()
            //    .GroupBy(t => t.IdPrioridad)
            //    .Select(g => new { Id = g.Key, Count = g.Count() })
            //    .ToList();

            //vm.PriorityLabels = prioridades.Select(p => p.NombrePrioridad).ToArray();
            //vm.PriorityCounts = prioridades.Select(p => prioridadCounts.FirstOrDefault(pc => pc.Id == p.IdPrioridad)?.Count ?? 0).ToArray();

            var days = 7;
            var today = DateTime.Today;
            var from = today.AddDays(-(days - 1));

            var created = _db.Tiquetes.AsNoTracking()
                .Where(t => t.CreatedAt >= from)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var waiting = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnEsperaDelUsuario)
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
            var esperaList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                esperaList.Add(waiting.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels.ToArray();
            vm.TrendCreados = creadosList.ToArray();
            vm.TrendEspera = esperaList.ToArray();
            vm.TrendCancelados = canceladosList.ToArray();

            return Json(vm);
        }

        public IActionResult Index()
        {

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/login");
            }

            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("AdminDashboard");
            }
            var role = User?.Identity != null && User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                : null;

            var counts = _db.Tiquetes.AsNoTracking()
                .GroupBy(t => t.IdEstatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var vm = new SASA.ViewModels.Home.DashboardViewModel
            {
                Abiertos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Creado)?.Count ?? 0,
                EnProgreso = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProceso)?.Count ?? 0,
                Resueltos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)?.Count ?? 0,
                Cancelados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)?.Count ?? 0,
                EnEsperaDelUsuario = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnEsperaDelUsuario)?.Count ?? 0,
                Rol = role
            };
            //var prioridades = _db.Prioridades.AsNoTracking().OrderBy(p => p.IdPrioridad).ToList();
            //var prioridadCounts = _db.Tiquetes.AsNoTracking()
            //    .GroupBy(t => t.IdPrioridad)
            //    .Select(g => new { Id = g.Key, Count = g.Count() })
            //    .ToList();

            //vm.PriorityLabels = prioridades.Select(p => p.NombrePrioridad).ToArray();
            //vm.PriorityCounts = prioridades.Select(p => prioridadCounts.FirstOrDefault(pc => pc.Id == p.IdPrioridad)?.Count ?? 0).ToArray();
            var days = 7;
            var today = DateTime.Today;
            var from = today.AddDays(-(days - 1));

            var created = _db.Tiquetes.AsNoTracking()
                .Where(t => t.CreatedAt >= from)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var waiting = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnEsperaDelUsuario)
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
            var esperaList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                esperaList.Add(waiting.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels.ToArray();
            vm.TrendCreados = creadosList.ToArray();
            vm.TrendEspera = esperaList.ToArray();
            vm.TrendCancelados = canceladosList.ToArray();

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AdminDashboard()
        {
            var porEstadoDto = await _tiqueteService.ObtenerTiquetesPorEstadoAsync();
            var ultimos7dias = await _tiqueteService.ObtenerTiquetesUltimos7DiasAsync();
            var viewModel = new DashboardAdminViewModel
            {
                TotalTiquetes = await _tiqueteService.ContarTiquetesAsync(),
                TotalInventario = await _inventarioService.ContarInventarioAsync(),
                PromedioResolucion = await _tiqueteService.PromedioResolucion(),
                PorEstado = porEstadoDto
                    .Select(p => new TiquetesPorEstadoViewModel
                    {
                        Estado = p.Estado,
                        Cantidad = p.Cantidad
                    })
                    .ToList(),
                Ultimos7Dias = ultimos7dias
                    .Select(d => new TiquetesPorDiaViewModel
                    {
                        Cantidad = d.Cantidad,
                        Fecha = d.Fecha
                    })
                    .ToList()
            };


            return View(viewModel);
        }

    }
}
