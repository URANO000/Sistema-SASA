using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("idNotificacion")]
        public long IdNotificacion { get; set; }

        [Required]
        [MaxLength(450)]
        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("idTiquete")]
        public int IdTiquete { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("tipoEvento")]
        public string TipoEvento { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [Column("leida")]
        public bool Leida { get; set; } = false;

        [Required]
        [Column("fechaCreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
