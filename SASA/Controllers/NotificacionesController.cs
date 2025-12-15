using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class NotificacionesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalle(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public IActionResult Auditoria()
        {
            return View();
        }

    }
}
