using Microsoft.AspNetCore.Identity;

namespace DataAccess.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }

        public string? NoEmpleado { get; set; }
        public string? Cedula { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        public bool Hijos { get; set; }
        public int? CantidadHijos { get; set; }

        public string? CorreoEmpresa { get; set; }
        public string? CorreoPersonal { get; set; }

        public bool Estado { get; set; }
        public string? SessionId { get; set; }
        public DateTime? LastActivityUtc { get; set; }
    }
}