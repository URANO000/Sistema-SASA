using BusinessLogic.Servicios.Rol;
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
        private readonly IRolService _rolService;

        public UsuarioController(IUsuarioService usuarioService, IRolService rolService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuariosDTO = await _usuarioService.ObtenerUsuariosAsync();
            var roles = await _rolService.ObtenerRolesAsync();

            var usuarios = usuariosDTO.Select(u => new UsuarioListaViewModel
            {
                Id = u.Id!,
                NombreCompleto = NombreCompletoHelper(u), //utilizo el helper
                Departamento = u.Departamento,
                Puesto = u.Puesto,
                CorreoEmpresa = u.CorreoEmpresa,
                Estado = u.Estado ? "Activo" : "Inactivo",
                Rol = u.Roles != null && u.Roles.Any()
            ? string.Join(", ", u.Roles)
            : "SIN ROL" //Por si no tiene rol, no dejarlo en nulo
            }).ToList();

            var model = new UsuarioIndexViewModel
            {
                Usuarios = usuarios,
                RolesDisponibles = roles //Esto es porque el modal existe en Index
            };

            return View(model);
        }

        private static string NombreCompletoHelper(ListaUsuarioDto dto)
        {
            //Juntar para el nombre completo
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
        public async Task<IActionResult> Details(string id)
        {
            //Check de que existe el id
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            //Obtener del servicio
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
                return NotFound();

            var model = new UsuarioDetalleViewModel
            {
                Id = usuario.Id!,
                NombreCompleto = NombreCompletoHelper(usuario),
                Departamento = usuario.Departamento,
                Puesto = usuario.Puesto,
                CorreoEmpresa = usuario.CorreoEmpresa,
                Estado = usuario.Estado ? "Activo" : "Inactivo",
                Rol = usuario.Roles != null && usuario.Roles.Any()
                    ? string.Join(", ", usuario.Roles)
                    : "SIN ROL"
            };

            return View(model);
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

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CrearUsuarioDto dto)
        {
            // Implement edit logic here
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

    }
}
