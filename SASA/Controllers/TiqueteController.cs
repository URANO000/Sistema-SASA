using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;

namespace SASA.Controllers
{
    [RequireAuth]
    public class TiqueteController : Controller
    {
        //GET: TiqueteController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
