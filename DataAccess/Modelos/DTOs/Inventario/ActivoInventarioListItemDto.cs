namespace DataAccess.Modelos.DTOs.Inventario
{
    public class ActivoInventarioListItemDto
    {
        public int IdActivo { get; set; }
        public string NumeroActivo { get; set; } = "";
        public string? NombreMaquina { get; set; }
        public string? SerieServicio { get; set; }

        public int IdTipoActivo { get; set; }
        public int IdEstadoActivo { get; set; }

        public string? TipoActivoNombre { get; set; }
        public string? EstadoActivoNombre { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}