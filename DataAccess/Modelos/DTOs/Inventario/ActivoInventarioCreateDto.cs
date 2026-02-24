namespace BusinessLogic.Modelos.DTOs.Inventario
{
    public class ActivoInventarioCreateDto
    {
        public string NumeroActivo { get; set; } = "";
        public string NombreMaquina { get; set; } = "";

        public int IdTipoActivo { get; set; }
        public int IdEstadoActivo { get; set; }

        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? SerieServicio { get; set; }
        public string? DireccionMAC { get; set; }
        public string? SistemaOperativo { get; set; }

        public int? IdTipoLicencia { get; set; }
        public string? ClaveLicencia { get; set; }
    }
}