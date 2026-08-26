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

        // Indica si el usuario actual tiene permiso para ver el detalle completo del tiquete
        public bool TieneAccesoTiquete { get; set; } = true;

        // Valores para preservar filtros / paginación al volver al índice
        public string? ReturnQ { get; set; }
        public string? ReturnTipo { get; set; }
        public string? ReturnEstado { get; set; }
        // Fecha en formato yyyy-MM-dd para pasar por query string
        public string? ReturnFecha { get; set; }
        public int ReturnPagina { get; set; } = 1;
        public int ReturnTamanoPagina { get; set; } = 10;

    }
}
