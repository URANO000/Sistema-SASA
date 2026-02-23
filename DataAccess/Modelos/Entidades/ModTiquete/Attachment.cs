using DataAccess.Identity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("Attachment")]
    public class Attachment
    {
        [Key]
        [Column("idAttachment")]
        public int IdAttachment { get; set; }

        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [Column("file",TypeName = "varbinary(max)")]
        public byte[]? File { get; set; }

        //No está mapeado -----------------------------
        [NotMapped]
        public IFormFile? AttachmentFile { get; set; }

        [Column("fileName")]
        public string? FileName { get; set; }

        [Column("uploadedBy")]
        public required string UploadedBy { get; set; }

        [Column("uploadedAt")]
        public required DateTime UploadedAt { get; set; }

        [Column("fileSize")]
        public required double FileSize { get; set; }

        //Navigation properties -> Relaciones entre entidades
        //[ForeignKey(nameof(IdTiquete))]
        public Tiquete? Tiquete { get; set; }

        //[ForeignKey(nameof(UploadedBy))]
        public ApplicationUser? Usuario { get; set; } //UploadedBy


    }
}
