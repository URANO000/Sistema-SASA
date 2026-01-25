using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("CATEGORIA")]
    public class Categoria
    {
        [Key]
        [Column("idCategoria")]
        public int IdCategoria { get; set; }

        [Column("nombreCategoria")]
        public string NombreCategoria { get; set; }

        //Collection -> Relacion con tiquetes
        public ICollection<Tiquete>? Tiquete { get; set; }
        public ICollection<Cola>? Cola { get; set; }
    }
}
