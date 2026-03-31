using DataAccess.Modelos.DTOs.Inventario;

namespace SASA.ViewModels.Inventario
{
    public class MantenimientoIndexViewModel
    {
        public List<MantenimientoActivoListItemDto> Items { get; set; } = new();

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public bool HasPrev => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
    }
}