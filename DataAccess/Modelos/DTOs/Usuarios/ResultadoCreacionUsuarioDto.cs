namespace DataAccess.Modelos.DTOs.Usuarios
{
    public class ResultadoCreacionUsuarioDto
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string EmailConfirmationToken { get; set; } = "";
    }
}
