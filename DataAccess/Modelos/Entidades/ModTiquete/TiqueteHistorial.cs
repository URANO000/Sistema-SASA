using DataAccess.Identity;
using DataAccess.Modelos.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("TiqueteHistorial")]
    public class TiqueteHistorial
    {
        [Key]
        [Column("idHistorial")]
        public int IdHistorial { get; set; }
        [Column("idTiquete")]
        public int IdTiquete { get; set; }
        [Column("tipoEvento")]
        public TipoEventoTiquete TipoEvento { get; set; }
        [Column("campoAfectado")]
        public string? CampoAfectado { get; set; }
        [Column("valorAnterior")]
        public string? ValorAnterior { get; set; }
        [Column("valorNuevo")]
        public string? ValorNuevo { get; set; }
        [Column("descripcionEvento")]
        public string? DescripcionEvento { get; set; }

        [Column("performedAt")]
        public DateTime PerformedAt { get; set; }
        [Column("performedBy")]
        public required string PerformedBy { get; set; }

        //Propiedades de navegación
        [ForeignKey(nameof(IdTiquete))]
        public Tiquete Tiquete { get; set; } = null!;

        [ForeignKey(nameof(PerformedBy))]
        public ApplicationUser User { get; set; } = null!;

    }
}
