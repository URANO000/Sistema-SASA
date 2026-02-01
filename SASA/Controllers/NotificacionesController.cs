using System.Security.Claims;
using BusinessLogic.Servicios.Notificaciones;
using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly INotificacionService _service;

        public NotificacionesController(INotificacionService service)
        {
            _service = service;
        }

        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IActionResult> Index(int pagina = 1, int tamanoPagina = 10)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            var result = await _service.ObtenerPorUsuarioAsync(userId, pagina, tamanoPagina);
            return View(result);
        }

        // Escenario 3: acceso al detalle (marca leída y navega al tiquete)
        public async Task<IActionResult> Abrir(long id)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            var n = await _service.ObtenerPorIdAsync(id, userId);
            if (n == null) return NotFound();

            await _service.MarcarComoLeidaAsync(id, userId);

            return RedirectToAction("Details", "Tiquete", new { id = n.IdTiquete });
        }

        [HttpPost]
        public async Task<IActionResult> AlternarLeida(long idNotificacion)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            await _service.AlternarLeidaAsync(idNotificacion, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarcarTodasComoLeidas()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return RedirectToAction("Login", "Account");

            await _service.MarcarTodasComoLeidasAsync(userId);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Contador()
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return Json(0);

            var total = await _service.ContarNoLeidasAsync(userId);
            return Json(total);
        }
    }
}
