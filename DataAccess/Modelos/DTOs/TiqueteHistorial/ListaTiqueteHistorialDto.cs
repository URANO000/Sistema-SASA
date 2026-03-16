using DataAccess.Modelos.Enums;

namespace DataAccess.Modelos.DTOs.TiqueteHistorial
{
    public class ListaTiqueteHistorialDto
    {
        public int IdHistorial { get; set; }
        public int IdTiquete { get; set; }
        public string? TipoEvento { get; set; }
        public string? CampoAfectado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string? DescripcionEvento { get; set; }
        public DateTime PerformedAt { get; set; }
        public required string PerformedBy { get; set; }
    }
}
