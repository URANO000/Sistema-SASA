namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class CrearTiqueteUsuarioDto
    {
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public required int IdCategoria { get; init; }

    }
}
