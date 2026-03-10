namespace DataAccess.Modelos.DTOs.SubCategoria
{
    public class CrearSubCategoriaDto
    {
        public int IdCategoria { get; set; }
        public int IdPrioridad { get; set; }
        public string NombreSubCategoria { get; set; } = null!;
    }
}