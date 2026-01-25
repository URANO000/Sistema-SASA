using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    public class Usuario /*: IdentityUser*/
    {
        [Key]
        [Column("Id")]
        public string Id { get; set; }

        [Column("primerNombre")]
        public required string PrimerNombre { get; set; }

        [Column("primerApellido")]
        public required string PrimerApellido { get; set; }

        [Column("departamento")]
        public string? Departamento { get; set; }

        [Column("puesto")]
        public string? Puesto { get; set; }

        [Column("jefeId")]

        public string? JefeId { get; set; }

        [Column("estado")]
        public bool Estado { get; set; }

        [Column("lastActivityUtc")]
        public DateTime LastActivityUtc { get; set; }

        //Collection -> Relaciones uno a muchos
        public ICollection<Tiquete>? Tiquete { get; set; }
        public ICollection<Comentario>? Comentario { get; set; }
        public ICollection<Cola>? Cola { get; set; }
        public ICollection<Attachment>? Attachment { get; set; }
    }
}
