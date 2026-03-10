namespace DataAccess.Modelos.DTOs.SubCategoria
{
    public class EditarSubCategoriaDto
    {
        public int IdSubCategoria { get; set; }
        public int IdCategoria { get; set; }
        public int IdPrioridad { get; set; }
        public string NombreSubCategoria { get; set; } = null!;
    }
}