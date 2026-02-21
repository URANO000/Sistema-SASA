using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("SubCategoria")]
    public class SubCategoria
    {
        [Key]
        [Column("idSubCategoria")]
        public int IdSubCategoria { get; set; }

        [Column("idCategoria")]
        public int IdCategoria { get; set; }

        [Column("idPrioridad")]
        public int IdPrioridad { get; set; }

        [Column("nombreSubCategoria")]
        public required string NombreSubCategoria { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey(nameof(IdPrioridad))]
        public Prioridad Prioridad { get; set; } = null!;
    }
}
