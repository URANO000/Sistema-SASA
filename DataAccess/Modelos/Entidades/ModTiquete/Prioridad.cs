using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("Prioridad")]
    public class Prioridad
    {
        [Key]
        [Column("idPrioridad")]
        public int IdPrioridad { get; set; }

        [Column("nombrePrioridad")]
        public string NombrePrioridad { get; set; } = null!;

        [Column("duracionMinutos")]
        public int DuracionMinutos { get; set; }
//Collection -> Relacion entre entidades
        public ICollection<SubCategoria>? SubCategorias { get; set; }
    }
}


