using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Prioridad")]
    public class Prioridad
    {
        [Key]
        [Column("idPrioridad")]
        public int IdPrioridad { get; set; }

        [Required]
        [Column("nombrePrioridad")]
        [StringLength(100)]
        public string NombrePrioridad { get; set; } = null!;

        //Colección de tiquetes asociados a esta prioridad
        public ICollection<Tiquete> Tiquetes { get; set; } = new List<Tiquete>();
    }

}
