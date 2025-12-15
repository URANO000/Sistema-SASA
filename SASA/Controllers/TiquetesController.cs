using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class TiquetesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Crear()
        {
            return View();
        }
        public IActionResult Detalle(int id)
        {
            ViewBag.Id = id;
            return View();
        }

    }
}
