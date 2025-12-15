using Microsoft.AspNetCore.Mvc;
using SASA.Filters;

namespace SASA.Controllers
{
    [RequireAuth]
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

    }
}
