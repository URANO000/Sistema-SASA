using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.TiqueteHistoriales
{
    public class TiqueteHistorialFiltroViewModel
    {
        public string? Search { get; set; }
        public string? TipoEvento { get; set; }          
        public IEnumerable<SelectListItem>? TipoEventoOptions { get; set; } // new: options for dropdown
        public DateTime? Fecha { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool TieneAnterior => PageNumber > 1;
        public bool TieneSiguiente => PageNumber < TotalPages;
    }
}
