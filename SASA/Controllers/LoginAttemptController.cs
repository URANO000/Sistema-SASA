using BusinessLogic.Servicios.Autenticacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SASA.ViewModels.Auth;

namespace SASA.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class LoginAttemptController : Controller
    {
        private readonly ILoginAttemptService _loginAttemptService;

        public LoginAttemptController(ILoginAttemptService loginAttemptService)
        {
            _loginAttemptService = loginAttemptService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(LoginAttemptFiltroViewModel filtros)
        {
            var filtroDto = new LoginAttemptFiltroDto
            {
                EmailIngresado = filtros.EmailIngresado,
                Exitoso = filtros.Exitoso,
                Fecha = filtros.Fecha,
                FechaDesde = filtros.FechaDesde,
                FechaHasta = filtros.FechaHasta,
                Page = filtros.Page <= 0 ? 1 : filtros.Page,
                PageSize = filtros.PageSize <= 0 ? 10 : filtros.PageSize
            };

            var resultado = await _loginAttemptService.ObtenerIntentosAsync(filtroDto);

            var viewModel = new LoginAttemptIndexViewModel
            {
                Filtros = filtros,
                Items = resultado.Items,
                TotalCount = resultado.TotalCount
            };

            return View(viewModel);
        }
    }
}
