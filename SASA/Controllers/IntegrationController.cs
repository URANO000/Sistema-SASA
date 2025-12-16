using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class IntegrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Preview()
        {
            return View();
        }

        public IActionResult Result()
        {
            return View();
        }
    }
}
