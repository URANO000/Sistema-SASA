using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;

namespace SASA.Controllers
{
    [RequireAuth]
    public class UsuarioController : Controller
    {
        //Referencia a los servicios (Inyección de dependencias)
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CrearUsuarioDto dto)
        {
            if(!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _usuarioService.AgregarUsuarioAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        public IActionResult Edit()
        {
            return View();
        }

    }
}
