namespace DataAccess.Modelos.DTOs.Inventario
{
    public class ActivoInventarioFiltroDto
    {
        public string? Texto { get; set; }
        public int? IdEstadoActivo { get; set; }
        public int? IdTipoActivo { get; set; }

        // Paginación
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Orden
        // Valores esperados: "Codigo", "Nombre", "Tipo", "Estado"
        public string SortBy { get; set; } = "Codigo";

        // Valores esperados: "asc" o "desc"
        public string SortDir { get; set; } = "asc";
    }
}