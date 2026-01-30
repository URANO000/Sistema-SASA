using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.Usuario;

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


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuariosDTO = await _usuarioService.ObtenerUsuariosAsync();

            var model = usuariosDTO.Select(u => new UsuarioListaViewModel
            {
                Id = u.Id!,
                NombreCompleto = NombreCompletoHelper(u),
                Departamento = u.Departamento,
                Puesto = u.Puesto,
                CorreoEmpresa = u.CorreoEmpresa,
                Estado = u.Estado ? "Activo" : "Inactivo",
                Rol = u.Roles != null && u.Roles.Any()
            ? string.Join(", ", u.Roles)
            : "SIN ROL"
            }).ToList();

            return View(model);
        }

        private static string NombreCompletoHelper(ListaUsuarioDto dto)
        {
            return string.Join(" ",
                new[]
                {
                    dto.PrimerNombre,
                    dto.SegundoNombre,
                    dto.PrimerApellido,
                    dto.SegundoApellido
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        [HttpGet]
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
            if (!ModelState.IsValid)
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

        public IActionResult Cancel()
        {
            return View();
        }

    }
}
