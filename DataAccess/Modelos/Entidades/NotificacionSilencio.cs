using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Modelos.Entidades
{
    [Table("NotificacionSilencio")]
    [PrimaryKey(nameof(UserId), nameof(idTiquete), nameof(fechaInicio))]
    public class NotificacionSilencio
    {
        [MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        public int idTiquete { get; set; }

        public DateTime fechaInicio { get; set; } = DateTime.Now;

        public DateTime? fechaFin { get; set; }
    }
}
