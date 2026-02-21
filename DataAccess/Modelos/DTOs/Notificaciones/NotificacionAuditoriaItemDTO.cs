using System;

namespace DataAccess.Modelos.DTOs.Notificaciones
{
    public class NotificacionAuditoriaItemDTO
    {
        public long IdNotificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public int IdTiquete { get; set; }
        public string Destinatario { get; set; } = string.Empty;
        public string Canal { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }
}
