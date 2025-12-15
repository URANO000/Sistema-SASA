using Microsoft.AspNetCore.Mvc;
using SASA.Filters;

namespace SASA.Controllers
{
    [RequireAuth]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }



    }
}
