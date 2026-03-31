using DataAccess.Modelos.DTOs.Inventario;

namespace SASA.ViewModels.Inventario
{
    public class ReporteGeneralInventarioViewModel
    {
        public int TotalActivos { get; set; }
        public int ActivosActivos { get; set; }
        public int EnMantenimiento { get; set; }

        public List<ActivoTelefonoInventarioListItemDto> Items { get; set; } = new();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public bool HasPrev => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}