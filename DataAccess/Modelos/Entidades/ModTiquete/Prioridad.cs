using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("PRIORIDAD")]
    public class Prioridad
    {
        [Key]
        [Column("idPrioridad")]
        public int IdPrioridad { get; set; }

        [Column("nombrePrioridad")]
        public string NombrePrioridad { get; set; }

        [Column("duracionMinutos")]
        public int DuracionMinutos { get; set; }

        //Collection -> Relacion entre entidades
        public ICollection<SubCategoria>? SubCategorias { get; set; }
    }
}
