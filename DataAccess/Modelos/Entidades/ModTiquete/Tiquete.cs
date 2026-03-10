using DataAccess.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("Tiquete")]
    public class Tiquete
    {
        [Key]
        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [Column("asunto")]
        public required string Asunto { get; set; } = null!;

        [Column("descripcion")]
        public string Descripcion { get; set; } = null!;

        [Column("resolucion")]
        public string? Resolucion { get; set; }

        [Column("idEstatus")]
        public required int IdEstatus { get; set; }

        [Column("idCategoria")]
        public required int IdCategoria { get; set; }

        [Column("idSubCategoria")]
        public required int IdSubCategoria { get; set; }

        [Column("ordenCola")]
        public int OrdenCola { get; set; }

        [Column("idAsignee")]
        public string? IdAsignee { get; set; }

        [Column("idReportedBy")]
        public string? IdReportedBy { get; set; }

        [Column("departamento")]
        public string? Departamento { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updatedBy")]
        public string? UpdatedBy { get; set; }

        //Navigation properties
        [ForeignKey(nameof(IdEstatus))]
        public Estatus Estatus { get; set; } = null!;

        [ForeignKey(nameof(IdCategoria))]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey(nameof(IdSubCategoria))]
        public SubCategoria SubCategoria { get; set; } = null!;

        [ForeignKey(nameof(IdAsignee))]
        public ApplicationUser? Asignee { get; set; }

        [ForeignKey(nameof(IdReportedBy))]
        public ApplicationUser? ReportedBy { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public ApplicationUser? UpdatedByUser { get; set; }


        //Collection -> Relaciones uno a muchos
        public ICollection<Attachment>? Attachments { get; set; }
        public ICollection<TiqueteHistorial>? TiqueteHistoriales { get; set; }
        public ICollection<Avance>? Avances { get; set; }

    }

}