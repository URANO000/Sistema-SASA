using BusinessLogic.Servicios.Inventario;
using BusinessLogic.Servicios.Tiquetes;
using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SASA.Filters;
using SASA.Models;
using SASA.ViewModels.Home;
using SASA.Helpers;
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

   
            var isAdmin = string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase);
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var tiquetesQuery = _db.Tiquetes.AsNoTracking().AsQueryable();
            if (!isAdmin && !string.IsNullOrEmpty(userId))
            {
                tiquetesQuery = tiquetesQuery.Where(t => t.IdReportedBy == userId || t.IdAsignee == userId);
            }

            var counts = tiquetesQuery
                .GroupBy(t => t.IdEstatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var vm = new DashboardViewModel
            {
                Abiertos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Creado)?.Count ?? 0,
                EnProgreso = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProceso)?.Count ?? 0,
                Resueltos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)?.Count ?? 0,
                Cancelados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)?.Count ?? 0,
                EnEsperaDelUsuario = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnEsperaDelUsuario)?.Count ?? 0,
                Rol = role
            };
            var prioridades = _db.Prioridades.AsNoTracking().ToList();
            var orderMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["critica"] = 0,
                ["crítica"] = 0,
                ["alta"] = 1,
                ["media"] = 2,
                ["baja"] = 3
            };
            prioridades = prioridades
                .OrderBy(p => orderMap.TryGetValue((p.NombrePrioridad ?? string.Empty).Trim().ToLowerInvariant(), out var r) ? r : 99)
                .ThenBy(p => p.IdPrioridad)
                .ToList();

            var durationMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["baja"] = 168,
                ["media"] = 24,
                ["alta"] = 4,
                ["critica"] = 1,
                ["crítica"] = 1
            };

            vm.PriorityLabels = prioridades.Select(p => p.NombrePrioridad).ToArray();
            vm.PriorityCounts = prioridades.Select(p =>
            {
                var name = (p.NombrePrioridad ?? string.Empty).Trim();
                return durationMap.TryGetValue(name, out var hours) ? hours : 0;
            }).ToArray();
            var priorityTicketCountsDict = (from t in tiquetesQuery
                                            join sc in _db.SubCategorias.AsNoTracking() on t.IdSubCategoria equals sc.IdSubCategoria into scj
                                            from sc in scj.DefaultIfEmpty()
                                            where sc != null && sc.IdPrioridad.HasValue
                                            group t by sc.IdPrioridad into g
                                            select new { PrioridadId = g.Key, Count = g.Count() })
                                         .ToDictionary(x => x.PrioridadId.Value, x => x.Count);

            vm.PriorityTicketCounts = prioridades.Select(p => priorityTicketCountsDict.ContainsKey(p.IdPrioridad) ? priorityTicketCountsDict[p.IdPrioridad] : 0).ToArray();
            vm.PriorityDisplayLabels = prioridades.Select((p, i) =>
            {
                var hours = vm.PriorityCounts.ElementAtOrDefault(i);
                var dur = hours > 0 ? DateTimeHelper.FormatearDuracionHoras(hours) : string.Empty;
                return string.IsNullOrEmpty(dur) ? p.NombrePrioridad : $"{p.NombrePrioridad} ({dur})";
            }).ToArray();

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

            var resolved = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var inProgress = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProceso)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            // resolved and inProgress already computed above

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
            var resueltosList = new List<int>();
            var esperaList = new List<int>();
            var enprogresoList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                resueltosList.Add(resolved.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                esperaList.Add(waiting.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                enprogresoList.Add(inProgress.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels.ToArray();
            vm.TrendCreados = creadosList.ToArray();
            vm.TrendResueltos = resueltosList.ToArray();
            vm.TrendEnProgreso = enprogresoList.ToArray();
            vm.TrendEspera = esperaList.ToArray();
            vm.TrendCancelados = canceladosList.ToArray();

            // Return vm plus some normalized aliases so client-side finds consistent keys
            return Json(new
            {
                Abiertos = vm.Abiertos,
                EnProgreso = vm.EnProgreso,
                Resueltos = vm.Resueltos,
                Cancelados = vm.Cancelados,
                EnEsperaDelUsuario = vm.EnEsperaDelUsuario,
                EnEspera = vm.EnEsperaDelUsuario,
                enEspera = vm.EnEsperaDelUsuario,
                PriorityLabels = vm.PriorityLabels,
                PriorityCounts = vm.PriorityCounts,
                PriorityDisplayLabels = vm.PriorityDisplayLabels,
                PriorityTicketCounts = vm.PriorityTicketCounts,
                TrendLabels = vm.TrendLabels,
                TrendCreados = vm.TrendCreados,
                TrendResueltos = vm.TrendResueltos,
                TrendEnProgreso = vm.TrendEnProgreso,
                TrendEspera = vm.TrendEspera,
                TrendCancelados = vm.TrendCancelados
            });
        }

        public IActionResult Index()
        {

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Redirect("/login");
            }

            if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "Cola");
            }
            var role = User?.Identity != null && User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
                : null;


            var isAdmin = string.Equals(role, "Administrador", StringComparison.OrdinalIgnoreCase);
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var tiquetesQuery = _db.Tiquetes.AsNoTracking().AsQueryable();
            if (!isAdmin && !string.IsNullOrEmpty(userId))
            {
                tiquetesQuery = tiquetesQuery.Where(t => t.IdReportedBy == userId || t.IdAsignee == userId);
            }

            var counts = tiquetesQuery
                .GroupBy(t => t.IdEstatus)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            var vm = new DashboardViewModel
            {
                Abiertos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Creado)?.Count ?? 0,
                EnProgreso = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProceso)?.Count ?? 0,
                Resueltos = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)?.Count ?? 0,
                Cancelados = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Cancelado)?.Count ?? 0,
                EnEsperaDelUsuario = counts.FirstOrDefault(x => x.Status == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnEsperaDelUsuario)?.Count ?? 0,
                Rol = role
            };


            var prioridades = _db.Prioridades.AsNoTracking().ToList();

            var orderMap2 = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["critica"] = 0,
                ["crítica"] = 0,
                ["alta"] = 1,
                ["media"] = 2,
                ["baja"] = 3
            };
            prioridades = prioridades
                .OrderBy(p => orderMap2.TryGetValue((p.NombrePrioridad ?? string.Empty).Trim().ToLowerInvariant(), out var r2) ? r2 : 99)
                .ThenBy(p => p.IdPrioridad)
                .ToList();
            var priorityCountsDictInit = (from t in tiquetesQuery
                                      join sc in _db.SubCategorias.AsNoTracking() on t.IdSubCategoria equals sc.IdSubCategoria into scj
                                      from sc in scj.DefaultIfEmpty()
                                      where sc != null && sc.IdPrioridad.HasValue
                                      group t by sc.IdPrioridad into g
                                      select new { PrioridadId = g.Key, Count = g.Count() })
                                     .ToDictionary(x => x.PrioridadId.Value, x => x.Count);

            var durationMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                ["baja"] = 168,
                ["media"] = 24,
                ["alta"] = 4,
                ["critica"] = 1,
                ["crítica"] = 1
            };

            vm.PriorityLabels = prioridades.Select(p => p.NombrePrioridad).ToArray();
            vm.PriorityCounts = prioridades.Select(p =>
            {
                var name = (p.NombrePrioridad ?? string.Empty).Trim();
                return durationMap.TryGetValue(name, out var hours) ? hours : 0;
            }).ToArray();
            var priorityTicketCountsDictInit = (from t in tiquetesQuery
                                            join sc in _db.SubCategorias.AsNoTracking() on t.IdSubCategoria equals sc.IdSubCategoria into scj
                                            from sc in scj.DefaultIfEmpty()
                                            where sc != null && sc.IdPrioridad.HasValue
                                            group t by sc.IdPrioridad into g
                                            select new { PrioridadId = g.Key, Count = g.Count() })
                                         .ToDictionary(x => x.PrioridadId.Value, x => x.Count);

            vm.PriorityTicketCounts = prioridades.Select(p => priorityTicketCountsDictInit.ContainsKey(p.IdPrioridad) ? priorityTicketCountsDictInit[p.IdPrioridad] : 0).ToArray();
            vm.PriorityDisplayLabels = prioridades.Select((p, i) =>
            {
                var hours = vm.PriorityCounts.ElementAtOrDefault(i);
                var dur = hours > 0 ? DateTimeHelper.FormatearDuracionHoras(hours) : string.Empty;
                return string.IsNullOrEmpty(dur) ? p.NombrePrioridad : $"{p.NombrePrioridad} ({dur})";
            }).ToArray();
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

            var resolved = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.Resuelto)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var inProgress = _db.Tiquetes.AsNoTracking()
                .Where(t => t.UpdatedAt.HasValue && t.UpdatedAt.Value.Date >= from && t.IdEstatus == (int)DataAccess.Modelos.Enums.TiqueteEstatus.EnProceso)
                .GroupBy(t => t.UpdatedAt!.Value.Date)
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
            var resueltosList = new List<int>();
            var enprogresoList = new List<int>();
            var esperaList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                resueltosList.Add(resolved.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                enprogresoList.Add(inProgress.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                esperaList.Add(waiting.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels.ToArray();
            vm.TrendCreados = creadosList.ToArray();
            vm.TrendResueltos = resueltosList.ToArray();
            vm.TrendEnProgreso = enprogresoList.ToArray();
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
