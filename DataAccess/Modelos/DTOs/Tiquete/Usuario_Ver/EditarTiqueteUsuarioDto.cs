namespace DataAccess.Modelos.DTOs.Tiquete.Usuario_Ver
{
    public class EditarTiqueteUsuarioDto
    {
        public required int IdTiquete { get; init; }
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public required string UpdatedBy { get; init; }
    }
}
