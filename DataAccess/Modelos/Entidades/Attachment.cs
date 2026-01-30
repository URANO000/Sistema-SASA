using DataAccess.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("ATTACHMENT")]
    public class Attachment
    {
        [Key]
        [Column("idAttachment")]
        public int IdAttachment { get; set; }

        [ForeignKey("idTiquete")]
        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [ForeignKey("idComentario")]
        [Column("idComentario")]
        public int? IdComentario { get; set; }

        [Column("filePath")]
        public string? FilePath { get; set; }

        [Column("fileName")]
        public string? FileName { get; set; }

        [ForeignKey("uploadedBy")]
        [Column("uploadedBy")]
        public required int UploadedBy { get; set; }

        [Column("uploadedAt")]
        public required DateTime UploadedAt { get; set; }

        [Column("fileSize")]
        public required double FileSize { get; set; }

        [Column("mimeType")]
        public required string MimeType { get; set; }

        //Navigation properties -> Relaciones entre entidades
        public Comentario? Comentario { get; set; }
        public Tiquete? Tiquete { get; set; }

        public ApplicationUser? Usuario { get; set; } //UploadedBy


    }
}
