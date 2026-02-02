using BusinessLogic.Servicios.Correo;
using BusinessLogic.Servicios.Rol;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using SASA.Filters;
using SASA.ViewModels.Usuario;
using System.Text;

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

                // Armar payload igual que AccountController: "userId|identityToken" en Base64Url
                var raw = $"{result.UserId}|{result.EmailConfirmationToken}";
                var payload = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(raw));

                // Construir link (la acción espera SOLO {token})
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
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            //Llamar al servicio para obtener el usuario
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _rolService.ObtenerRolesAsync(); //cargar roles

            var model = new UsuarioEditarViewModel
            {
                Id = usuario.Id!,
                PrimerNombre = usuario.PrimerNombre,
                SegundoNombre = usuario.SegundoNombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                CorreoEmpresa = usuario.CorreoEmpresa,
                Departamento = usuario.Departamento,
                Puesto = usuario.Puesto,
                Estado = usuario.Estado,
                Rol = usuario.Roles?.FirstOrDefault() ?? string.Empty,

                RolesDisponibles = roles
            .Select(r => new SelectListItem
            {
                Value = r,
                Text = r
            })
            .ToList()
            };

            //Para activar/Desactivar
            ViewData["UserId"] = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioEditarViewModel model)
        {
            //Validamos el modelo
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                               .Where(x => x.Value!.Errors.Any())
                               .ToDictionary(
                                   k => k.Key,
                                   v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                               )
                });
            }

            //Si todo está bien, mapear a DTO para transferencia
            var dto = new EditarUsuarioDto
            {
                Id = model.Id,
                PrimerNombre = model.PrimerNombre,
                SegundoNombre = model.SegundoNombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                CorreoEmpresa = model.CorreoEmpresa,
                Departamento = model.Departamento,
                Puesto = model.Puesto,
                Rol = model.Rol
            };

            //Ahora, llamar al servicio con try and catch
            try
            {
                await _usuarioService.ActualizarUsuarioAsync(dto);
                return Json(new
                {
                    success = true
                });

            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        _form = new[] { ex.Message }
                    }
                });
            }
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
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }
            try
            {
                await _usuarioService.ActivarUsuarioAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
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

        //Helpers

        private async Task<IReadOnlyList<SelectListItem>> CargarRolesAsync()
        {
            var roles = await _rolService.ObtenerRolesAsync();

            return roles
                .Select(r => new SelectListItem
                {
                    Value = r,
                    Text = r
                })
                .ToList();
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


        //public async Task<IActionResult> CargarIndexConErrores(CrearUsuarioViewModel model)
        //{
        //    //Es similar al Index, pero cargando el modelo con errores
        //    //Ya que utilizo vistas compartidas (UsuarioIndexViewModel)
        //    var usuariosDTO = await _usuarioService.ObtenerUsuariosAsync();
        //    var roles = await _rolService.ObtenerRolesAsync();

        //    var usuarios = usuariosDTO.Select(u => new UsuarioListaViewModel
        //    {
        //        Id = u.Id!,
        //        NombreCompleto = NombreCompletoHelper(u), //utilizo el helper
        //        Departamento = u.Departamento,
        //        Puesto = u.Puesto,
        //        CorreoEmpresa = u.CorreoEmpresa,
        //        Estado = u.Estado ? "Activo" : "Inactivo",
        //        Rol = u.Roles != null && u.Roles.Any()
        //    ? string.Join(", ", u.Roles)
        //    : "SIN ROL" //Por si no tiene rol, no dejarlo en nulo
        //    }).ToList();

        //    var indexModel = new UsuarioIndexViewModel
        //    {
        //        Usuarios = usuarios,
        //        RolesDisponibles = roles,
        //        CrearUsuario = model
        //    };

        //    return View("Index", indexModel);
        //}
    }
}
