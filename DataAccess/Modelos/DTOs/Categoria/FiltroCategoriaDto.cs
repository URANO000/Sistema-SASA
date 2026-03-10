namespace DataAccess.Modelos.DTOs.Categoria
{
    public class FiltroCategoriaDto
    {
        public string? Buscar { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}