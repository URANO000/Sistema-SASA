using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class IntegrationController : Controller
    {
        // GET: /Integration
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Integration/Upload
        public IActionResult Upload()
        {
            return View();
        }

        // GET: /Integration/Validation
        public IActionResult Validation()
        {
            return View();
        }

        // GET: /Integration/History
        public IActionResult History()
        {
            return View();
        }
    }
}
