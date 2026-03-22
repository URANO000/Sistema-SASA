namespace DataAccess.Modelos.DTOs.SubCategoria
{
    public class ListaSubCategoriaDto
    {
        public int IdSubCategoria { get; set; }
        public string NombreSubCategoria { get; set; } = null!;
        public int? IdCategoria { get; set; }
        public string? NombreCategoria { get; set; }
        public int? IdPrioridad { get; set; }
        public string? NombrePrioridad { get; set; }

        public int? DuracionMinutos { get; set; }
        // Propiedad para mostrar la duración formateada en la vista
        public string? DuracionDisplay { get; set; }
    }
}