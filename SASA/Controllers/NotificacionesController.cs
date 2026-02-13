using BusinessLogic.Servicios.Notificaciones;
using DataAccess; 
using DataAccess.Modelos.DTOs.Notificaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SASA.ViewModels.Notificaciones;

namespace SASA.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly INotificacionService _service;
        private readonly ApplicationDbContext _db;

        public NotificacionesController(INotificacionService service, ApplicationDbContext db)
        {
            _service = service;
            _db = db;
        }


        private async Task<string?> GetUserIdRealAsync()
        {

            var claimId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(claimId))
                return claimId;


            var email = HttpContext.Session.GetString("auth_email");
            if (string.IsNullOrWhiteSpace(email))
                return null;


            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            return user?.Id;
        }

        public async Task<IActionResult> Index(int pagina = 1, int tamanoPagina = 10)
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            var result = await _service.ObtenerPorUsuarioAsync(userId, pagina, tamanoPagina);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AlternarLeida(long idNotificacion)
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            await _service.AlternarLeidaAsync(idNotificacion, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarcarTodasComoLeidas()
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            await _service.MarcarTodasComoLeidasAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Contador()
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return Json(0);

            var total = await _service.ContarNoLeidasAsync(userId);
            return Json(total);
        }

        public async Task<IActionResult> Detalle(long id)
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            var n = await _service.ObtenerPorIdAsync(id, userId);
            if (n == null) return NotFound();

            if (!n.Leida)
                await _service.MarcarComoLeidaAsync(id, userId);

            static string Shorten(string? text, int max)
            {
                if (string.IsNullOrWhiteSpace(text)) return "—";
                text = text.Trim();
                return text.Length <= max ? text : text.Substring(0, max) + "…";
            }

            var t = await _db.Tiquetes
                .AsNoTracking()
                .Where(x => x.IdTiquete == n.IdTiquete)
                .Select(x => new
                {
                    x.IdTiquete,
                    x.Asunto,
                    x.Descripcion,
                    x.Resolucion,

                    CreatedAt = x.CreatedAt,

                    PrioridadObj = EF.Property<object>(x, "IdPrioridad"),

                    x.IdEstatus,
                    x.IdCategoria,
                    x.IdCola,
                    x.IdAsignee,
                    x.IdReportedBy
                })
                .FirstOrDefaultAsync();

            var vm = new NotificacionDetalleViewModel
            {
                Notificacion = n,
                IdTiquete = n.IdTiquete,
                CreatedAt = null,
            };

            if (t != null)
            {

                vm.CreatedAt = t.CreatedAt;

                int? prioridadId = t.PrioridadObj is int p ? p : (int?)null;

                vm.Asunto = t.Asunto;

                vm.Estatus = await _db.Estatuses.AsNoTracking()
                    .Where(e => e.IdEstatus == t.IdEstatus)
                    .Select(e => e.NombreEstatus)
                    .FirstOrDefaultAsync() ?? "—";

                if (prioridadId.HasValue)
                {
                    vm.Prioridad = await _db.Prioridades.AsNoTracking()
                        .Where(p0 => p0.IdPrioridad == prioridadId.Value)
                        .Select(p0 => p0.NombrePrioridad)
                        .FirstOrDefaultAsync() ?? "—";
                }
                else
                {
                    vm.Prioridad = "—";
                }

                vm.Categoria = await _db.Categorias.AsNoTracking()
                    .Where(c => c.IdCategoria == t.IdCategoria)
                    .Select(c => c.NombreCategoria)
                    .FirstOrDefaultAsync() ?? "—";

                vm.Cola = await _db.Colas.AsNoTracking()
                    .Where(c => c.IdCola == t.IdCola)
                    .Select(c => c.NombreCola)
                    .FirstOrDefaultAsync() ?? "—";

                if (!string.IsNullOrWhiteSpace(t.IdAsignee))
                {
                    vm.AsignadoA = await _db.Users.AsNoTracking()
                        .Where(u => u.Id == t.IdAsignee)
                        .Select(u => u.Email ?? u.UserName)
                        .FirstOrDefaultAsync() ?? "—";
                }

                if (!string.IsNullOrWhiteSpace(t.IdReportedBy))
                {
                    vm.ReportadoPor = await _db.Users.AsNoTracking()
                        .Where(u => u.Id == t.IdReportedBy)
                        .Select(u => u.Email ?? u.UserName)
                        .FirstOrDefaultAsync() ?? "—";
                }

                vm.DescripcionPreview = Shorten(t.Descripcion, 220);
                vm.ResolucionPreview = Shorten(t.Resolucion, 220);
                var silenciadoHasta = await _service.ObtenerSilencioActivoAsync(userId, n.IdTiquete);
                vm.EstaSilenciado = silenciadoHasta.HasValue;
                vm.SilenciadoHasta = silenciadoHasta;

            }

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> SilenciarTiquete(int idTiquete, int horas, long? returnId = null)
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            var permitido = new[] { 1, 8, 24 };
            if (!permitido.Contains(horas)) horas = 1;

            await _service.SilenciarTiqueteAsync(userId, idTiquete, horas);

            if (returnId.HasValue)
                return RedirectToAction(nameof(Detalle), new { id = returnId.Value });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ReactivarSilencio(int idTiquete, long? returnId = null)
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            await _service.ReactivarSilencioAsync(userId, idTiquete);

            if (returnId.HasValue)
                return RedirectToAction(nameof(Detalle), new { id = returnId.Value });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Auditoria(
            string? q,
            string? tipo,
            string? estado,
            DateTime? fecha,
            int pagina = 1,
            int tamanoPagina = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 10;

            var query = _db.Notificaciones.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(n =>
                    n.Mensaje.Contains(q) ||
                    n.TipoEvento.Contains(q) ||
                    n.IdTiquete.ToString().Contains(q) ||
                    _db.Users.Any(u => u.Id == n.UserId &&
                        ((u.Email != null && u.Email.Contains(q)) ||
                         (u.UserName != null && u.UserName.Contains(q))))
                );
            }


            if (!string.IsNullOrWhiteSpace(tipo) && tipo != "Tipo")
            {
                query = query.Where(n => n.TipoEvento == tipo);
            }


            if (!string.IsNullOrWhiteSpace(estado) && estado != "Estado")
            {
                if (estado == "Leida") query = query.Where(n => n.Leida);
                if (estado == "NoLeida") query = query.Where(n => !n.Leida);
            }

            if (fecha.HasValue)
            {
                var d = fecha.Value.Date;
                var d2 = d.AddDays(1);
                query = query.Where(n => n.FechaCreacion >= d && n.FechaCreacion < d2);
            }

            query = query.OrderByDescending(n => n.FechaCreacion);

            var total = await query.CountAsync();

            var elementos = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(n => new NotificacionAuditoriaItemViewModel
                {
                    IdNotificacion = n.IdNotificacion,
                    FechaCreacion = n.FechaCreacion,
                    TipoEvento = n.TipoEvento,
                    IdTiquete = n.IdTiquete,
                    Destinatario = _db.Users
                        .Where(u => u.Id == n.UserId)
                        .Select(u => u.Email ?? u.UserName ?? "—")
                        .FirstOrDefault() ?? "—",
                    Canal = "In-App",
                    Estado = n.Leida ? "Leida" : "NoLeida"
                })
                .ToListAsync();

            var vm = new NotificacionAuditoriaViewModel
            {
                Q = q,
                Tipo = tipo,
                Estado = estado,
                Fecha = fecha,
                Pagina = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = total,
                Elementos = elementos
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AuditoriaDetalle(long id)
        {
            var n = await _db.Notificaciones
                .AsNoTracking()
                .Where(x => x.IdNotificacion == id)
                .Select(x => new NotificacionDTO
                {
                    IdNotificacion = x.IdNotificacion,
                    IdTiquete = x.IdTiquete,
                    TipoEvento = x.TipoEvento,
                    Mensaje = x.Mensaje,
                    Leida = x.Leida,
                    FechaCreacion = x.FechaCreacion
                })
                .FirstOrDefaultAsync();

            if (n == null) return NotFound();

            static string Shorten(string? text, int max)
            {
                if (string.IsNullOrWhiteSpace(text)) return "—";
                text = text.Trim();
                return text.Length <= max ? text : text.Substring(0, max) + "…";
            }

            var t = await _db.Tiquetes
                .AsNoTracking()
                .Where(x => x.IdTiquete == n.IdTiquete)
                .Select(x => new
                {
                    x.IdTiquete,
                    x.Asunto,
                    x.Descripcion,
                    x.Resolucion,

                    CreatedAt = x.CreatedAt,

                    PrioridadObj = EF.Property<object>(x, "IdPrioridad"),

                    x.IdEstatus,
                    x.IdCategoria,
                    x.IdCola,
                    x.IdAsignee,
                    x.IdReportedBy
                })
                .FirstOrDefaultAsync();

            var vm = new NotificacionDetalleViewModel
            {
                Notificacion = n,
                IdTiquete = n.IdTiquete,
                CreatedAt = null,
                EstaSilenciado = false,
                SilenciadoHasta = null
            };

            if (t != null)
            {
                vm.CreatedAt = t.CreatedAt;

                int? prioridadId = t.PrioridadObj is int p ? p : (int?)null;

                vm.Asunto = t.Asunto;

                vm.Estatus = await _db.Estatuses.AsNoTracking()
                    .Where(e => e.IdEstatus == t.IdEstatus)
                    .Select(e => e.NombreEstatus)
                    .FirstOrDefaultAsync() ?? "—";

                if (prioridadId.HasValue)
                {
                    vm.Prioridad = await _db.Prioridades.AsNoTracking()
                        .Where(p0 => p0.IdPrioridad == prioridadId.Value)
                        .Select(p0 => p0.NombrePrioridad)
                        .FirstOrDefaultAsync() ?? "—";
                }
                else
                {
                    vm.Prioridad = "—";
                }

                vm.Categoria = await _db.Categorias.AsNoTracking()
                    .Where(c => c.IdCategoria == t.IdCategoria)
                    .Select(c => c.NombreCategoria)
                    .FirstOrDefaultAsync() ?? "—";

                vm.Cola = await _db.Colas.AsNoTracking()
                    .Where(c => c.IdCola == t.IdCola)
                    .Select(c => c.NombreCola)
                    .FirstOrDefaultAsync() ?? "—";

                if (!string.IsNullOrWhiteSpace(t.IdAsignee))
                {
                    vm.AsignadoA = await _db.Users.AsNoTracking()
                        .Where(u => u.Id == t.IdAsignee)
                        .Select(u => u.Email ?? u.UserName)
                        .FirstOrDefaultAsync() ?? "—";
                }

                if (!string.IsNullOrWhiteSpace(t.IdReportedBy))
                {
                    vm.ReportadoPor = await _db.Users.AsNoTracking()
                        .Where(u => u.Id == t.IdReportedBy)
                        .Select(u => u.Email ?? u.UserName)
                        .FirstOrDefaultAsync() ?? "—";
                }

                vm.DescripcionPreview = Shorten(t.Descripcion, 220);
                vm.ResolucionPreview = Shorten(t.Resolucion, 220);
            }
            return View(vm);
        }


    }
}
