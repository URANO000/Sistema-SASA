using BusinessLogic.Servicios.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.Perfil;
using System.Security.Claims;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var usuario = await _usuarioService.ObtenerPerfilAsync(userId);

            if (usuario == null)
                return NotFound();

            var viewModel = new PerfilViewModel
            {
                NombreCompleto = string.Join(" ", new[]
                {
                    usuario.PrimerNombre,
                    usuario.SegundoNombre,
                    usuario.PrimerApellido,
                    usuario.SegundoApellido
                }.Where(x => !string.IsNullOrWhiteSpace(x))),
                CorreoEmpresa = usuario.CorreoEmpresa,
                Departamento = usuario.Departamento,
                Puesto = usuario.Puesto,
                Rol = usuario.Rol,
                Estado = usuario.Estado ? "Activo" : "Inactivo"
            };

            return View(viewModel);
        }
    }
}