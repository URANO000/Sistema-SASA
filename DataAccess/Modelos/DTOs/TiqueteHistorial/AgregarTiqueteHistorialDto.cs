using DataAccess.Modelos.Enums;

namespace DataAccess.Modelos.DTOs.TiqueteHistorial
{
    public class AgregarTiqueteHistorialDto
    {
        public int IdTiquete { get; set; }
        public TipoEventoTiquete TipoEvento { get; set; }
        public string? CampoAfectado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string? DescripcionEvento { get; set; }
        public required string PerformedBy { get; set; }

    }
}
