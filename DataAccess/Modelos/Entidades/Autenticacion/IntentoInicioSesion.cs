using DataAccess.Identity;

namespace DataAccess.Modelos.Entidades.Autenticacion
{
    public class IntentoInicioSesion
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; }

        public string EmailIngresado { get; set; } = default!;
        public string? UserId { get; set; }
        public bool Exitoso { get; set; }
        public string? MotivoFallo { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public ApplicationUser? User { get; set; }


    }
}
