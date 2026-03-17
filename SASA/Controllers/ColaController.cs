using BusinessLogic.Servicios.Tiquetes;
using DataAccess.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.Tiquete.Cola;

namespace SASA.Controllers
{
    [RequireAuth]
    public class ColaController : Controller
    {
        //Lo único que es diferente es el controlador, pero toda la lógica y consistencia de datos pertenece a tiquetes
        //La separación de controladores es una decisión meramente personal, pero está sujeto a cambios de ser necesario
        private readonly ITiqueteService _service;
        private readonly UserManager<ApplicationUser> _userManager;
        public ColaController(ITiqueteService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize (Roles = "Administrador")]
        public async Task<IActionResult> Index(string tab = "Cola Personal")
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var colaDto = await _service.GetColaPersonalAsync(currentUser.Id);
            var colaAssigneeDto = await _service.GetColasGlobalAsync();

            var viewModel = new ColaIndexViewModel
            {
                TabActiva = tab,
                Personal = colaDto.Select(t => new ColaPersonalViewModel
                {
                    IdTiquete = t.IdTiquete,
                    Asunto = t.Asunto,
                    PosicionCola = t.PosicionCola,
                    OrdenCola = t.OrdenCola,
                    Estatus = t.Estatus,
                    Categoria = t.Categoria,
                    SubCategoria = t.SubCategoria,
                    Prioridad = t.Prioridad,
                    Asignee = t.Asignee,
                    CreatedAt = t.CreatedAt
                }).ToList(),

                Global = colaAssigneeDto.Select(g => new ColaGlobalViewModel
                {
                    AssigneeId = g.AssigneeId,
                    AssigneeCorreo = g.AssigneeCorreo,

                    Colas = g.Colas.Select(t => new ColaPersonalViewModel
                    {
                        IdTiquete = t.IdTiquete,
                        Asunto = t.Asunto,
                        PosicionCola = t.PosicionCola,
                        Categoria = t.Categoria,
                        SubCategoria = t.SubCategoria,
                        Prioridad = t.Prioridad
                    }).ToList()
                }).ToList()
            };
            return View(viewModel);
        }
    }
}
