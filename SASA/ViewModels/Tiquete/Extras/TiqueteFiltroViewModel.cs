using SASA.ViewModels.Tiquete.Extras;

namespace SASA.ViewModels.Tiquete.Filtro
{
    public class TiqueteFiltroViewModel : TiqueteFormViewModel
    {
        public string? Search { get; set; }
        public string? Estatus { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public bool TieneAnterior => PageNumber > 1;
        public bool TieneSiguiente => PageNumber < TotalPages;
    }
}
