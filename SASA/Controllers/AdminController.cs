using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
