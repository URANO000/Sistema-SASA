using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("TIQUETE")]
    public class Tiquete
    {
        [Key]
        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [Column("asunto")]
        public required string Asunto { get; set; }

        [Column("descripcion")]
        public required string Descripcion { get; set; }

        [ForeignKey("idEstatus")]
        [Column("idEstatus")]
        public int IdEstatus { get; set; }

        [ForeignKey("idPrioridad")]
        [Column("idPrioridad")]
        public int IdPrioridad { get; set; }

        [ForeignKey("idCategoria")]
        [Column("idCategoria")]
        public int IdCategoria { get; set; }

        [ForeignKey("idCola")]
        [Column("idCola")]
        public int IdCola { get; set; }

        [ForeignKey("idAsignee")]
        [Column("idAsignee")]
        public int IdAsignee { get; set; }

        [ForeignKey("idReportedBy")]
        [Column("idReportedBy")]
        public int IdReportedBy { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Column("resolucion")]
        public string? Resolucion { get; set; }

        //Navigation properties -> Relaciones entre entidades
        public required Estatus Estatus {get;set;}
        public Prioridad? Prioridad { get; set; }
        public Categoria Categoria { get; set; }
        public Cola? Cola { get; set; }

        public Usuario? Asignee { get; set; }
        public Usuario? ReportedBy { get; set; }

        //Collection -> Relaciones uno a muchos
        public ICollection<Comentario>? Comentarios { get; set; }
        public ICollection<Attachment>? Attachments { get; set; }


    }
}
