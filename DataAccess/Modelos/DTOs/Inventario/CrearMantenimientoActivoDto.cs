namespace DataAccess.Modelos.DTOs.Inventario
{
    public class CrearMantenimientoActivoDto
    {
        public int IdActivo { get; set; }
        public DateTime FechaMantenimiento { get; set; }
        public string TipoMantenimiento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}