using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using SASA.ViewModels.Shared;

namespace SASA.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuUsuarioViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new MenuUsuarioViewModel();

            var user = HttpContext?.User as ClaimsPrincipal;
            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                vm.EstaAutenticado = true;

                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                ApplicationUser? appUser = null;
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    appUser = await _userManager.FindByIdAsync(userId);
                }

                if (appUser != null)
                {
                    var given = appUser.PrimerNombre?.Trim();
                    var surname = appUser.PrimerApellido?.Trim();
                    vm.NombreMostrar = (!string.IsNullOrWhiteSpace(given) || !string.IsNullOrWhiteSpace(surname))
                        ? ($"{given} {surname}").Trim()
                        : appUser.Email ?? appUser.UserName ?? "Usuario";

                    var roles = await _userManager.GetRolesAsync(appUser);
                    vm.Rol = roles.FirstOrDefault() ?? "—";
                }
                else
                {
                    vm.NombreMostrar = user.Identity.Name ?? "Usuario";
                }
            }
            else
            {
                vm.EstaAutenticado = false;
                vm.NombreMostrar = "Invitado";
                vm.Rol = "—";
            }

            return View(vm);
        }
    }
}
