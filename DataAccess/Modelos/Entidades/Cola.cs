
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("COLA")]
    public class Cola
    {
        [Key]
        [Column("idCola")]
        public int IdCola { get; set; }

        [ForeignKey("idCategoria")]
        [Column("idCategoria")]
        public int idCategoria { get; set; }

        [Column("nombreCola")]
        public string NombreCola { get; set; }

        [Column("descripcionCola")]
        public string DescripcionCola { get; set; }

        [Column("isActive")]
        public bool IsActive { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("createdBy")]
        [Column("createdBy")]
        public string CreatedBy { get; set; }

        //Navigation properties -> Relaciones entre entidades

        //NOT YET: AÑADIR REFERENCIA A USUARIO
        public Categoria? Categoria { get; set; }

        //Collection -> Relaciones uno a muchos
        public Tiquete? Tiquete { get; set; }
    }
}
