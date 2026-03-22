using BusinessLogic.Servicios.Helpers;
using BusinessLogic.Servicios.Tiquetes;
using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Tiquete.Colas;
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
        private readonly IHelper _helper;
        public ColaController(ITiqueteService service, UserManager<ApplicationUser> userManager, IHelper helper)
        {
            _service = service;
            _userManager = userManager;
            _helper = helper;
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
                Personal = colaDto.Select(t =>
                {
                    (string? restante, string? excedido, bool atrasado) =
                        t.DuracionMinutos.HasValue
                            ? _helper.Calcular(t.CreatedAt, t.DuracionMinutos.Value)
                            : (null, null, false);

                    return new ColaPersonalViewModel
                    {
                        IdTiquete = t.IdTiquete,
                        Asunto = t.Asunto,
                        PosicionCola = t.PosicionCola,
                        OrdenCola = t.OrdenCola,
                        Estatus = t.Estatus,
                        Categoria = t.Categoria,
                        SubCategoria = t.SubCategoria,
                        Prioridad = t.Prioridad,
                        DuracionMinutos = t.DuracionMinutos,
                        Asignee = t.Asignee,
                        CreatedAt = t.CreatedAt,

                        TiempoRestante = restante,
                        TiempoExcedido = excedido,
                        EstaAtrasado = atrasado
                    };
                }).ToList(),

                Global = colaAssigneeDto.Select(g => new ColaGlobalViewModel
                {
                    AssigneeId = g.AssigneeId,
                    AssigneeNombre = g.AssigneeNombre,

                    Colas = g.Colas.Select(t =>
                    {
                        (string? restante, string? excedido, bool atrasado) =
                            t.DuracionMinutos.HasValue
                                ? _helper.Calcular(t.CreatedAt, t.DuracionMinutos.Value)
                                : (null, null, false);

                        return new ColaPersonalViewModel
                        {
                            IdTiquete = t.IdTiquete,
                            Asunto = t.Asunto,
                            PosicionCola = t.PosicionCola,
                            Categoria = t.Categoria,
                            SubCategoria = t.SubCategoria,
                            Prioridad = t.Prioridad,
                            DuracionMinutos = t.DuracionMinutos,
                            CreatedAt = t.CreatedAt,

                            TiempoRestante = restante,
                            TiempoExcedido = excedido,
                            EstaAtrasado = atrasado
                        };
                    }).ToList()
                }).ToList()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Reordenar([FromBody] ReordenarDto dto)
        {
            var nuevoOrden = await _service.ReordenarAsync(
               dto.IdTiquete,
               dto.OrdenAnterior,
               dto.OrdenSiguiente
           );

            return Ok(new { nuevoOrden });
        }
    }

}
