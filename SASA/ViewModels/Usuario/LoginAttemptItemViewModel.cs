namespace SASA.ViewModels.Usuario
{
    public class LoginAttemptItemViewModel
    {
        public DateTime FechaUtc { get; set; }

        public string FechaLocal { get; set; } = default!;
        public bool Exitoso { get; set; }
        public string? MotivoFallo { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string EmailIngresado { get; set; } = default!;
    }
}