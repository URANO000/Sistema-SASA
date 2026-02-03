using BusinessLogic.Servicios.Notificaciones;
using DataAccess; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            return View(n);
        }
    }
}
