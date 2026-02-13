namespace SASA.ViewModels.Notificaciones
{
    public class NotificacionAuditoriaItemViewModel
    {
        public long IdNotificacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string TipoEvento { get; set; } = "—";
        public int IdTiquete { get; set; }

        public string Destinatario { get; set; } = "—";
        public string Canal { get; set; } = "In-App";   
        public string Estado { get; set; } = "Enviada"; 
    }
}
