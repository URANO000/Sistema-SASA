namespace DataAccess.Modelos.DTOs.Integracion
{
    public class ValidacionImportacionDto
    {
        public int HistorialId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;

        public int TotalFilas { get; set; }
        public int FilasValidas { get; set; }
        public int FilasConError { get; set; }

        public List<FilaValidacionDto> Errores { get; set; } = new();
        public List<FilaImportacionActivosDto> FilasValidasPreview { get; set; } = new();
    }

    public class FilaValidacionDto
    {
        public int NumeroFila { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class FilaImportacionActivosDto
    {
        public int NumeroFila { get; set; }
        public string NumeroActivo { get; set; } = string.Empty;
        public string NombreMaquina { get; set; } = string.Empty;
        public string TipoEquipo { get; set; } = string.Empty;

        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? SerieServicio { get; set; }
        public string? DireccionMAC { get; set; }
        public string? SistemaOperativo { get; set; }

        public string? TipoLicencia { get; set; }
        public string? ClaveLicencia { get; set; }
    }
}