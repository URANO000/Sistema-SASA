namespace DataAccess.Modelos.DTOs.Inventario
{
    public class ActivoInventarioDetailDto
    {
        public int IdActivo { get; set; }
        public string NumeroActivo { get; set; } = "";
        public string? NombreMaquina { get; set; }

        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? SerieServicio { get; set; }
        public string? DireccionMAC { get; set; }
        public string? SistemaOperativo { get; set; }

        public int IdTipoActivo { get; set; }
        public int IdEstadoActivo { get; set; }

        public int? IdTipoLicencia { get; set; }
        public string? ClaveLicencia { get; set; }

        public string? TipoActivoNombre { get; set; }
        public string? EstadoActivoNombre { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}