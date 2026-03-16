using DataAccess.Modelos.DTOs.InventarioTelefono;

namespace SASA.ViewModels.InventarioTelefono
{
    public class TelefonoIndexViewModel
    {
        public List<ActivoTelefonoListItemDto> Items { get; set; } = new();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; } = 1;

        public string? Q { get; set; }

        public string SortBy { get; set; } = "Nombre";
        public string SortDir { get; set; } = "asc";

        public bool HasPrev => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        public string ToggleDir(string column)
        {
            if (SortBy == column)
                return SortDir == "asc" ? "desc" : "asc";

            return "asc";
        }

        public string SortIcon(string column)
        {
            if (SortBy != column) return "";
            return SortDir == "asc" ? "▲" : "▼";
        }
    }
}