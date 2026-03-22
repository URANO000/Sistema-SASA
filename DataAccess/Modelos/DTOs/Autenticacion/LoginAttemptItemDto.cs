namespace DataAccess.Modelos.DTOs.Autenticacion
{
    public class LoginAttemptItemDto
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; }
        public string EmailIngresado { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public bool Exitoso { get; set; }
        public string? MotivoFallo { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}