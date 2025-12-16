using Microsoft.AspNetCore.Mvc;
using SASA.Filters; 

namespace SASA.Controllers
{
    [RequireAuth]
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
