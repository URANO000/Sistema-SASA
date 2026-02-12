using DataAccess.Modelos.DTOs.Notificaciones;

namespace SASA.ViewModels.Notificaciones
{
    public class NotificacionDetalleViewModel
    {
        public NotificacionDTO Notificacion { get; set; } = null!;

        public int IdTiquete { get; set; }
        public string Asunto { get; set; } = "—";
        public string Estatus { get; set; } = "—";
        public string Prioridad { get; set; } = "—";
        public string Categoria { get; set; } = "—";
        public string Cola { get; set; } = "—";
        public string ReportadoPor { get; set; } = "—";
        public string AsignadoA { get; set; } = "—";
        public DateTime? CreatedAt { get; set; }

        public string DescripcionPreview { get; set; } = "—";
        public string ResolucionPreview { get; set; } = "—";
        public bool EstaSilenciado { get; set; }
        public DateTime? SilenciadoHasta { get; set; }

    }
}
