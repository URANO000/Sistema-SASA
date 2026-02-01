using BusinessLogic.Servicios.Correo;
using BusinessLogic.Servicios.Rol;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.Filters;
using SASA.ViewModels.Usuario;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

using System.Threading.Tasks;

namespace SASA.Controllers
{
    [RequireAuth]
    public class UsuarioController : Controller
    {
        //Referencia a los servicios (Inyección de dependencias)
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly ActivationEmailService _activationEmailService;


        public UsuarioController(IUsuarioService usuarioService, IRolService rolService, ActivationEmailService activationEmailService)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _activationEmailService = activationEmailService;
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


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CrearUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //Para AJAX, retornar modal parcial con errores de validación
                model.RolesDisponibles = (IReadOnlyList<SelectListItem>?)await _rolService.ObtenerRolesAsync();
                return PartialView("_AddModal", model);
            }

            //Mapeo a DTO
            var dto = new CrearUsuarioDto
            {
                PrimerNombre = model.PrimerNombre,
                SegundoNombre = model.SegundoNombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                CorreoEmpresa = model.CorreoEmpresa,
                Departamento = model.Departamento,
                Puesto = model.Puesto,
                Rol = model.Rol
            };

            try
            {
                var result = await _usuarioService.AgregarUsuarioAsync(dto);

                // ✅ Armar payload igual que AccountController: "userId|identityToken" en Base64Url
                var raw = $"{result.UserId}|{result.EmailConfirmationToken}";
                var payload = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(raw));

                // ✅ Construir link (la acción espera SOLO {token})
                var activationLink = Url.Action(
                    "ActivateAccount",
                    "Account",
                    new { token = payload },
                    protocol: Request.Scheme
                );

                if (string.IsNullOrWhiteSpace(activationLink))
                    throw new Exception("No se pudo construir el link de activación. Verifica la ruta /activate-account/{token}.");

                // Enviar correo de activación
                await _activationEmailService.SendActivationAsync(
                    result.Email,
                    activationLink
                );

                //Para AJAX
                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                //Si un error ocurre, regresar al Index con el modelo y el error, o se cae todo
                ModelState.AddModelError(string.Empty, ex.Message);
                model.RolesDisponibles = (IReadOnlyList<SelectListItem>?)await _rolService.ObtenerRolesAsync();
                return PartialView("_AddModal", model);

            }
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            ViewData["UserId"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, CrearUsuarioDto dto)
        {
            // Implement edit logic here
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            try
            {
                await _usuarioService.DesactivarUsuarioAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }


        //Helpers
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

        public async Task<IActionResult> CargarIndexConErrores(CrearUsuarioViewModel model)
        {
            //Es similar al Index, pero cargando el modelo con errores
            //Ya que utilizo vistas compartidas (UsuarioIndexViewModel)
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

            var indexModel = new UsuarioIndexViewModel
            {
                Usuarios = usuarios,
                RolesDisponibles = roles,
                CrearUsuario = model
            };

            return View("Index", indexModel);
        }
    }
}
