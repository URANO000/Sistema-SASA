using DataAccess.Modelos.DTOs.Inventario;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.Inventario
{
    public class InventarioIndexViewModel
    {
        public List<ActivoInventarioListItemDto> Items { get; set; } = new();

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public int TotalRecords { get; set; } = 0;

        public bool HasPrev => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;

        // Filtros
        public string? Q { get; set; }
        public int? EstadoId { get; set; }
        public int? TipoId { get; set; }

        // Ordenamiento (NUEVO)
        public string SortBy { get; set; } = "Codigo";
        public string SortDir { get; set; } = "asc";

        // Combos
        public SelectList? Estados { get; set; }
        public SelectList? Tipos { get; set; }

        // Helpers UI
        public string ToggleDir(string column)
        {
            // Si estoy ordenando por la misma columna, alterno
            if (string.Equals(SortBy, column, StringComparison.OrdinalIgnoreCase))
                return string.Equals(SortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";

            // Si cambio de columna, arranco asc
            return "asc";
        }

        public string SortIcon(string column)
        {
            if (!string.Equals(SortBy, column, StringComparison.OrdinalIgnoreCase)) return "";
            return string.Equals(SortDir, "asc", StringComparison.OrdinalIgnoreCase) ? "▲" : "▼";
        }
    }
}