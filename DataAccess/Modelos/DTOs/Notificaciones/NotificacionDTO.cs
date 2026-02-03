using System;

namespace DataAccess.Modelos.DTOs.Notificaciones
{
    public class NotificacionDTO
    {
        public long IdNotificacion { get; set; }
        public int IdTiquete { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
