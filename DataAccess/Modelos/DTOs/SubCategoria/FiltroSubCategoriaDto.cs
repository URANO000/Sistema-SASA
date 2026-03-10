namespace DataAccess.Modelos.DTOs.SubCategoria
{
    public class FiltroSubCategoriaDto
    {
        public string? Buscar { get; set; }
        public int? IdCategoria { get; set; }
        public int? IdPrioridad { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}