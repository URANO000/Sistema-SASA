using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class FormularioController : Controller
    {
        public IActionResult AddFormulario()
        {
            return View();

        }

        public IActionResult Formularios()
        {
            return View();
        }
    }
}
