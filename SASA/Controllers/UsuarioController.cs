using BusinessLogic.Servicios.Correo;
using BusinessLogic.Servicios.Rol;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SASA.Configuration;
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
        private readonly ICorreoNotificacionesService _correoNotificaciones;
        private readonly AppSettings _appSettings;

        public UsuarioController(IUsuarioService usuarioService, IRolService rolService, ICorreoNotificacionesService correoNotificaciones, IOptions<AppSettings> appOptions)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _correoNotificaciones = correoNotificaciones;
            _appSettings = appOptions.Value;
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (!ModelState.IsValid)
            {
                model.RolesDisponibles = (await _rolService.ObtenerRolesAsync())
                    .Select(r => new SelectListItem { Value = r, Text = r })
                    .ToList();

                return PartialView("_AddModal", model);
            }

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

            ResultadoCreacionUsuarioDto result;
            try
            {
                result = await _usuarioService.AgregarUsuarioAsync(dto);
            }
            catch (Exception ex)
            {
                if (isAjax)
                    return Json(new { success = false, message = ex.Message });

                ModelState.AddModelError(string.Empty, ex.Message);
                model.RolesDisponibles = await CargarRolesAsync();
                return PartialView("_AddModal", model);
            }

            var raw = $"{result.UserId}|{result.EmailConfirmationToken}";
            var payload = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(raw));

            var baseUrl = _appSettings.BaseUrl?.TrimEnd('/');
            var activationLink = $"{baseUrl}/activate-account/{payload}";


            if (string.IsNullOrWhiteSpace(activationLink))
            {
                if (isAjax)
                    return Json(new { success = true, warning = "Usuario creado, pero no se pudo construir el link de activación." });

                return RedirectToAction(nameof(Index));
            }

            var toName = $"{model.PrimerNombre} {model.PrimerApellido}".Trim();

            bool correoEnviado = await _correoNotificaciones.EnviarActivacionCuentaAsync(
                model.CorreoEmpresa,
                toName,
                activationLink
            );

            string? warning = correoEnviado
                ? null
                : "El usuario fue creado, pero ocurrió un problema enviando el correo de activación.";

            if (isAjax)
                return Json(new { success = true, warning });

            return RedirectToAction(nameof(Index));
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

    }
}
