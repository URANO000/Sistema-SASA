using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Estatus")]
    public class Estatus
    {
        [Key]
        [Column("idEstatus")]
        public int IdEstatus { get; set; }

        [Required]
        [Column("nombreEstatus")]
        [StringLength(100)]
        public string NombreEstatus { get; set; } = null!;

        //Collección -> Relacion entre entidades
        public ICollection<Tiquete> Tiquetes { get; set; } = new List<Tiquete>();
    }
}
