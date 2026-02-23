namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class CrearTiqueteDto
    {
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public required int IdCategoria { get; init; }
        public string? IdAsignee { get; init; }
    }
}