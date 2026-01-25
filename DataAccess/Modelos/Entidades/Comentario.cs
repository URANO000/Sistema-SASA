
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("COMENTARIO")]
    public class Comentario
    {
        [Key]
        [Column("idComentario")]
        public int IdComentario { get; set; }

        [ForeignKey("idTiquete")]
        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [ForeignKey("idAutor")]
        [Column("idAutor")]
        public int IdAutor { get; set; }

        [Column("textoComentario")]
        public int textoComentario { get; set; }

        [Column("createdAt")]
        public required string createdAt { get; set; }

        //Navigation properties -> Relaciones entre entidades
        public required Tiquete Tiquete { get; set; }
        public required Usuario Autor { get; set; }


    }
}
