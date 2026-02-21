using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("Estatus")]
    public class Estatus
    {
        [Key]
        [Column("idEstatus")]
        public int IdEstatus { get; set; }

        [Column("nombreEstatus")]
        public string NombreEstatus { get; set; }

        //Collection -> Relacion con tiquetes
        public ICollection<Tiquete>? Tiquete { get; set; }
    }
}
