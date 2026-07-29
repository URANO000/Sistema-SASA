using BusinessLogic.Servicios.Helpers;
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
        private readonly IHelper _helper;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, ITiqueteService tiqueteService, IInventarioService inventarioService,
            IHelper helper)
        {
            _logger = logger;
            _db = db;
            _tiqueteService = tiqueteService;
            _inventarioService = inventarioService;
            _helper = helper;
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

            var subcategoriasAll = (from sc in _db.SubCategorias.AsNoTracking()
                                     select new { sc.IdSubCategoria, sc.NombreSubCategoria }).ToList();

            var subcategoryCountsList = (from t in tiquetesQuery
                                         join sc in _db.SubCategorias.AsNoTracking() on t.IdSubCategoria equals sc.IdSubCategoria into scj
                                         from sc in scj.DefaultIfEmpty()
                                         where sc != null
                                         group t by new { sc.IdSubCategoria, sc.NombreSubCategoria } into g
                                         select new { Id = g.Key.IdSubCategoria, Name = g.Key.NombreSubCategoria, Count = g.Count() })
                                        .ToList();

   
            var countsDict = subcategoryCountsList.ToDictionary(x => x.Id, x => x.Count);
            var allWithCounts = subcategoriasAll
                .Select(s => new { Id = s.IdSubCategoria, Name = s.NombreSubCategoria, Count = countsDict.ContainsKey(s.IdSubCategoria) ? countsDict[s.IdSubCategoria] : 0 })
                .ToList();

            int topN = 5;
            var ordered = allWithCounts.OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToList();
            var top = ordered.Take(topN).ToList();
            var labels = top.Select(x => x.Name).ToList();
            var countsTop = top.Select(x => x.Count).ToList();

            if (ordered.Count > topN)
            {
                var othersTotal = ordered.Skip(topN).Sum(x => x.Count);
                labels.Add("Otros");
                countsTop.Add(othersTotal);
            }

            vm.SubcategoryDisplayLabels = labels.ToArray();
            vm.SubcategoryTicketCounts = countsTop.ToArray();
            vm.SubcategoryLabels = vm.SubcategoryDisplayLabels;
            vm.SubcategoryCounts = vm.SubcategoryTicketCounts;

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

            var labelsTrend = new List<string>();
            var creadosList = new List<int>();
            var resueltosList = new List<int>();
            var esperaList = new List<int>();
            var enprogresoList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labelsTrend.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                resueltosList.Add(resolved.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                esperaList.Add(waiting.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                enprogresoList.Add(inProgress.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labelsTrend.ToArray();
            vm.TrendAbiertos = creadosList.ToArray();
            vm.TrendResueltos = resueltosList.ToArray();
            vm.TrendEnProgreso = enprogresoList.ToArray();
            vm.TrendEspera = esperaList.ToArray();
            vm.TrendCancelados = canceladosList.ToArray();

            return Json(new
            {
                Abiertos = vm.Abiertos,
                EnProgreso = vm.EnProgreso,
                Resueltos = vm.Resueltos,
                Cancelados = vm.Cancelados,
                EnEsperaDelUsuario = vm.EnEsperaDelUsuario,
                EnEspera = vm.EnEsperaDelUsuario,
                enEspera = vm.EnEsperaDelUsuario,
                SubcategoryLabels = vm.SubcategoryLabels,
                SubcategoryCounts = vm.SubcategoryCounts,
                SubcategoryDisplayLabels = vm.SubcategoryDisplayLabels,
                SubcategoryTicketCounts = vm.SubcategoryTicketCounts,
                TrendLabels = vm.TrendLabels,
                TrendAbiertos = vm.TrendAbiertos,
                TrendCreados = vm.TrendAbiertos,
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
                return RedirectToAction("AdminDashboard", "Home");
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

            var subcategoriesAllView = (from sc in _db.SubCategorias.AsNoTracking()
                                        select new { sc.IdSubCategoria, sc.NombreSubCategoria }).ToList();

            var subcategoryCountsListView = (from t in tiquetesQuery
                                             join sc in _db.SubCategorias.AsNoTracking() on t.IdSubCategoria equals sc.IdSubCategoria into scj
                                             from sc in scj.DefaultIfEmpty()
                                             where sc != null
                                             group t by new { sc.IdSubCategoria, sc.NombreSubCategoria } into g
                                             select new { Id = g.Key.IdSubCategoria, Name = g.Key.NombreSubCategoria, Count = g.Count() })
                                            .ToList();

            var countsDictView = subcategoryCountsListView.ToDictionary(x => x.Id, x => x.Count);
            var allWithCountsView = subcategoriesAllView
                .Select(s => new { Id = s.IdSubCategoria, Name = s.NombreSubCategoria, Count = countsDictView.ContainsKey(s.IdSubCategoria) ? countsDictView[s.IdSubCategoria] : 0 })
                .ToList();

            var orderedView = allWithCountsView.OrderByDescending(x => x.Count).ThenBy(x => x.Name).ToList();
            var topView = orderedView.Take(5).ToList();
            var labelsView = topView.Select(x => x.Name).ToList();
            var countsView = topView.Select(x => x.Count).ToList();

            if (orderedView.Count > 5)
            {
                var othersTotalView = orderedView.Skip(5).Sum(x => x.Count);
                labelsView.Add("Otros");
                countsView.Add(othersTotalView);
            }

            vm.SubcategoryDisplayLabels = labelsView.ToArray();
            vm.SubcategoryTicketCounts = countsView.ToArray();
            vm.SubcategoryLabels = vm.SubcategoryDisplayLabels;
            vm.SubcategoryCounts = vm.SubcategoryTicketCounts;

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

            var labels2 = new List<string>();
            var creadosList = new List<int>();
            var resueltosList = new List<int>();
            var enprogresoList = new List<int>();
            var esperaList = new List<int>();
            var canceladosList = new List<int>();

            for (int i = 0; i < days; i++)
            {
                var d = from.AddDays(i);
                labels2.Add(d.ToString("MMM d"));
                creadosList.Add(created.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                resueltosList.Add(resolved.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                enprogresoList.Add(inProgress.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                esperaList.Add(waiting.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
                canceladosList.Add(cancelled.FirstOrDefault(x => x.Date == d)?.Count ?? 0);
            }

            vm.TrendLabels = labels2.ToArray();
            vm.TrendAbiertos = creadosList.ToArray();
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
            var tiquetesVencidosPorEstado = await _tiqueteService.ObtenerTiquetesVencidosPorEstadoAsync();
            var promedio = await _tiqueteService.PromedioResolucionAsync();
            if (double.IsNaN(promedio))
                promedio = 0;
            var viewModel = new DashboardAdminViewModel
            {
                TotalTiquetes = await _tiqueteService.ContarTiquetesAsync(),
                TotalInventario = await _inventarioService.ContarInventarioAsync(),
                PromedioResolucion = promedio,
                PromedioResolucionFormateado = _helper.FormatTiempo(
                TimeSpan.FromMinutes(promedio)),
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
                    .ToList(),
                TiquetesVencidosPorEstado = tiquetesVencidosPorEstado
                    .Select(v => new TiquetesPorEstadoViewModel
                    {
                        Estado = v.Estado,
                        Cantidad = v.Cantidad
                    })
                    .ToList()
                
            };


            return View(viewModel);
        }

    }
}
