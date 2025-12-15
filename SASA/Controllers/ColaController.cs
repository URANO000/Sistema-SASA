using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class ColaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
    }
}
