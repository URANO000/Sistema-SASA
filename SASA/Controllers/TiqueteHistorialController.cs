using BusinessLogic.Servicios.TiqueteHistoriales;
using DataAccess.Modelos.DTOs.TiqueteHistorial.Filtros;
using DataAccess.Modelos.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.ViewModels.TiqueteHistoriales;

namespace SASA.Controllers
{
    public class TiqueteHistorialController : Controller
    {
        private readonly ITiqueteHistorialService _service;
        public TiqueteHistorialController(ITiqueteHistorialService service)
        {
            _service = service;
        }


        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Index(TiqueteHistorialFiltroViewModel filtro)
        {
            //Primero, mapear viewmodel a dto para BLL
            var pageNumber = filtro.PageNumber <= 0 ? 1 : filtro.PageNumber;
            var pageSize = filtro.PageSize <= 0 ? 10 : filtro.PageSize;

            var filtroDto = new TiqueteHistorialFiltroDto
            {
                Search = filtro.Search,
                TipoEvento = filtro.TipoEvento,
                Fecha = filtro.Fecha,
                FechaInicio = filtro.FechaInicio,
                FechaFinal = filtro.FechaFinal,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            //Obtener historial
            var result = await _service.ListarHistorialAsync(filtroDto);

            //Mapeo de resultado a ViewModel para tabla (con filtros)
            var viewModel = new TiqueteHistorialIndexViewModel
            {
                TiqueteHistorial = result.Items.Select(t => new ListaTiqueteHistorialViewModel
                {
                    IdHistorial = t.IdHistorial,
                    IdTiquete = t.IdTiquete,
                    TipoEvento = t.TipoEvento,
                    CampoAfectado = t.CampoAfectado,
                    ValorAnterior = t.ValorAnterior,
                    ValorNuevo = t.ValorNuevo,
                    DescripcionEvento = t.DescripcionEvento,
                    PerformedAt = t.PerformedAt,
                    PerformedBy = t.PerformedBy
                }).ToList(),

                Filtro = new TiqueteHistorialFiltroViewModel
                {
                    Search = filtro.Search,
                    TipoEvento = filtro.TipoEvento,
                    Fecha = filtro.Fecha,
                    FechaInicio = filtro.FechaInicio,
                    FechaFinal = filtro.FechaFinal,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = result.TotalPages
                }
            };

            //Cargo el valor del filtro
            await CargarFiltrosAsync(viewModel.Filtro);
            
            return View(viewModel);
        }



        //-------------------------------HELPERS-------------------------------------------
        private Task CargarFiltrosAsync(TiqueteHistorialFiltroViewModel model)
        {
            var tipos = Enum.GetValues(typeof(TipoEventoTiquete))
                .Cast<TipoEventoTiquete>();

            model.TipoEventoOptions = tipos.Select(t => new SelectListItem
            {
                Value = ((int)t).ToString(),   
                Text = t.ToString(),           
                Selected = model.TipoEvento == ((int)t).ToString()
            });

            return Task.CompletedTask;
        }
    }

    
}
