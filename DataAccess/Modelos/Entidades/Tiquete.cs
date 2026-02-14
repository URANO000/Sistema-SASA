using DataAccess.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Tiquete")]
    public class Tiquete
    {
        [Key]
        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [Required]
        [Column("asunto")]
        [StringLength(150)]
        public string Asunto { get; set; } = null!;

        [Required]
        [Column("descripcion")]
        public string Descripcion { get; set; } = null!;

        [Required]
        [Column("idEstatus")]
        public int IdEstatus { get; set; }

        [Column("idPrioridad")]
        public int IdPrioridad { get; set; }

        [Required]
        [Column("idCategoria")]
        public int IdCategoria { get; set; }

        [Required]
        [Column("idCola")]
        public int IdCola { get; set; }

        [Column("idAsignee")]
        [StringLength(450)]
        public string? IdAsignee { get; set; }

        [Column("idReportedBy")]
        [StringLength(450)]
        public string? IdReportedBy { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("updatedBy")]
        public string? UpdatedBy { get; set; }

        [Column("resolucion")]
        public string? Resolucion { get; set; }

        // Navigation properties
        [ForeignKey(nameof(IdEstatus))]
        public Estatus Estatus { get; set; } = null!;

        [ForeignKey(nameof(IdPrioridad))]
        public Prioridad? Prioridad { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        public Categoria Categoria { get; set; } = null!;

        [ForeignKey(nameof(IdCola))]
        public Cola Cola { get; set; } = null!;

        [ForeignKey(nameof(IdAsignee))]
        public ApplicationUser? Asignee { get; set; }

        [ForeignKey(nameof(IdReportedBy))]
        public ApplicationUser? ReportedBy { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public ApplicationUser? UpdatedByUser { get; set; }


        //Collection -> Relaciones uno a muchos
        public ICollection<Comentario>? Comentarios { get; set; }
        public ICollection<Attachment>? Attachments { get; set; }

    }

}