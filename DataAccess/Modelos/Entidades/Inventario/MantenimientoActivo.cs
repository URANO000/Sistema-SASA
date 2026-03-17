namespace DataAccess.Modelos.Entidades.Inventario
{
    public class MantenimientoActivo
    {
        public int IdMantenimiento { get; set; }

        public int IdActivo { get; set; }

        public DateTime FechaMantenimiento { get; set; }

        public string TipoMantenimiento { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public DateTime FechaCreacion { get; set; }

        // navegación
        public ActivoInventario? Activo { get; set; }
    }
}   