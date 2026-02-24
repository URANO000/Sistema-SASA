using DataAccess.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.ModTiquete
{
    [Table("Avance")]
    public class Avance
    {
        [Key]
        [Column("idAvance")]
        public int IdAvance { get; set; }
        [Column("idTiquete")]
        public int IdTiquete { get; set; }
        [Column("idAutor")]
        public required string IdAutor { get; set; }
        [Column("textoAvance")]
        public required string TextoAvance { get; set; }
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(IdTiquete))]
        public Tiquete Tiquete { get; set; } = null!;
        [ForeignKey(nameof(IdAutor))]
        public ApplicationUser Autor { get; set; } = null!;
    }
}
