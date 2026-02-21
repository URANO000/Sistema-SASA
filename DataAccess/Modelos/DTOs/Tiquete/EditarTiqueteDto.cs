namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class EditarTiqueteDto
    {
        public required int IdTiquete { get; init; }
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public int IdCategoria { get; init; }
        public int IdEstatus { get; init; }
        public string? IdAsignee { get; init; }
        public string? Resolucion { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public required string UpdatedBy { get; init; }
    }
}
