using DataAccess.Modelos.Entidades.ModTiquete;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Column("primerNombre")]
        public string? PrimerNombre { get; set; }

        [Column("segundoNombre")]
        public string? SegundoNombre { get; set; }

        [Column("primerApellido")]
        public string? PrimerApellido { get; set; }

        [Column("segundoApellido")]
        public string? SegundoApellido { get; set; }

        [Column("noEmpleado")]
        public string? NoEmpleado { get; set; }

        [Column("cedula")]
        public string? Cedula { get; set; }

        [Column("fechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("hijos")]
        public bool Hijos { get; set; }

        [Column("cantidadHijos")]
        public int? CantidadHijos { get; set; }

        [Column("departamento")]
        public string? Departamento { get; set; }

        [Column("puesto")]
        public string? Puesto { get; set; }

        [Column("correoEmpresa")]
        public string? CorreoEmpresa { get; set; }

        [Column("correoPersonal")]
        public string? CorreoPersonal { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }

        [Column("sessionId")]
        public string? SessionId { get; set; }

        [Column("lastActivityUtc")]
        public DateTime? LastActivityUtc { get; set; }

        [Column("UserName")]
        public override string? UserName { get; set; }

        [Column("CreatedAt")]
        public DateTime? CreatedAt { get; set; }
        [Column("CreatedBy")]
        public string? CreatedById { get; set; }

        //Propiedades de navegación con sí mismo

        [ForeignKey(nameof(CreatedById))]
        public ApplicationUser? CreatedByUser { get; set; }

        public ICollection<ApplicationUser>? UsersCreated { get; set; }

        //Propiedades de navegación con tiquete
        public ICollection<Tiquete>? TiquetesAsignados { get; set; }
        public ICollection<Tiquete>? TiquetesReportados { get; set; }
        public ICollection<Tiquete>? TiquetesEditados { get; set; }
        public ICollection<Avance>? Avances { get; set; }
        public ICollection<Attachment>? Attachments { get; set; }
        public ICollection<TiqueteHistorial>? TiqueteHistoriales { get; set; }


        //Extra
        public string nombreCompleto =>
        string.Join(" ", new[] { PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido }
        .Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}