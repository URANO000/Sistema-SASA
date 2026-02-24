namespace BusinessLogic.Modelos.DTOs.Inventario
{
    public class ActivoInventarioFiltroDto
    {
        public string? Texto { get; set; }
        public int? IdTipoActivo { get; set; }
        public int? IdEstadoActivo { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}   