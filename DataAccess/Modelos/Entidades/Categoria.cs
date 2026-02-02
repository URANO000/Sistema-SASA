using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Categoria")]
    public class Categoria
    {
        [Key]
        [Column("idCategoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("nombreCategoria")]
        [StringLength(100)]
        public string NombreCategoria { get; set; } = null!;

        //Colecciones de navegación
        public ICollection<Cola> Colas { get; set; } = new List<Cola>();
        public ICollection<Tiquete> Tiquetes { get; set; } = new List<Tiquete>();
    }

}
