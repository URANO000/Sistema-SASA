using BusinessLogic.Servicios.Notificaciones;
using DataAccess.Modelos.DTOs.Notificaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SASA.ViewModels.Notificaciones;
using BusinessLogic.Servicios.Tiquetes;
using System.Security.Claims;

namespace SASA.Controllers
{
    public class NotificacionesController : Controller
    {

        private readonly INotificacionService _service;
        private readonly ITiqueteService _tiqueteService;

        public NotificacionesController(INotificacionService service, ITiqueteService tiqueteService)
        {
            _service = service;
            _tiqueteService = tiqueteService;
        }


        private async Task<string?> GetUserIdRealAsync()
        {

            var claimId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(claimId))
                return claimId;

            return null;
        }

        public async Task<IActionResult> Index(string? q, string? tipo, string? estado, DateTime? fecha, int pagina = 1, int tamanoPagina = 10)
        {
            var userId = await GetUserIdRealAsync();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 10;

            var result = await _service.ObtenerPorUsuarioAsync(userId, q, tipo, estado, fecha, pagina, tamanoPagina);

            var vm = new NotificacionIndexViewModel
            {
                Q = q,
                Tipo = tipo,
                Estado = estado,
                Fecha = fecha,
                Pagina = result.Pagina,
                TamanoPagina = result.TamanoPagina,
                TotalRegistros = result.TotalRegistros,
                Elementos = result.Elementos
            };

            try
            {
                vm.EsAdministrador = User?.IsInRole("Administrador") ?? false;
            }
            catch
            {
                vm.EsAdministrador = false;
            }

            return View(vm);
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

            var t = await _tiqueteService.ObtenerTiquetePorIdReadAsync(n.IdTiquete);

            var vm = new NotificacionDetalleViewModel
            {
                Notificacion = n,
                IdTiquete = n.IdTiquete,
                CreatedAt = null,
            };

            if (t != null)
            {
                vm.CreatedAt = t.CreatedAt;
                vm.Asunto = t.Asunto;
                vm.Estatus = t.Estatus ?? "—";
                vm.Categoria = t.Categoria ?? "—";
                vm.AsignadoA = !string.IsNullOrWhiteSpace(t.Assignee) ? t.Assignee : null;
                vm.ReportadoPor = !string.IsNullOrWhiteSpace(t.ReportedBy) ? t.ReportedBy : null;

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

            var result = await _service.ObtenerAuditoriaAsync(q, tipo, estado, fecha, pagina, tamanoPagina);

            var elementos = result.Elementos.Select(n => new NotificacionAuditoriaItemViewModel
            {
                IdNotificacion = n.IdNotificacion,
                FechaCreacion = n.FechaCreacion,
                TipoEvento = n.TipoEvento,
                IdTiquete = n.IdTiquete,
                Destinatario = n.Destinatario,
                Canal = n.Canal,
                Estado = n.Estado
            }).ToList();

            var vm = new NotificacionAuditoriaViewModel
            {
                Q = q,
                Tipo = tipo,
                Estado = estado,
                Fecha = fecha,
                Pagina = result.Pagina,
                TamanoPagina = result.TamanoPagina,
                TotalRegistros = result.TotalRegistros,
                Elementos = elementos
            };

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AuditoriaDetalle(long id)
        {
            var n = await _service.ObtenerPorIdParaAuditoriaAsync(id);
            if (n == null) return NotFound();

            static string Shorten(string? text, int max)
            {
                if (string.IsNullOrWhiteSpace(text)) return "—";
                text = text.Trim();
                return text.Length <= max ? text : text.Substring(0, max) + "…";
            }

            var t = await _tiqueteService.ObtenerTiquetePorIdReadAsync(n.IdTiquete);

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
                vm.Asunto = t.Asunto;
                vm.Estatus = t.Estatus ?? "—";
                vm.Categoria = t.Categoria ?? "—";
                vm.AsignadoA = t.Assignee ?? "—";
                vm.ReportadoPor = t.ReportedBy ?? "—";

                vm.DescripcionPreview = Shorten(t.Descripcion, 220);
                vm.ResolucionPreview = Shorten(t.Resolucion, 220);
            }

            return View(vm);
        }


    }
}
