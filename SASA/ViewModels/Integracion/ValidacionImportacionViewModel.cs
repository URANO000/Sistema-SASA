namespace SASA.ViewModels.Integracion
{
    public class ValidacionImportacionViewModel
    {
        public int HistorialId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;

        public int TotalFilas { get; set; }
        public int FilasValidas { get; set; }
        public int FilasConError { get; set; }

        public List<string> TiposEquipoDisponibles { get; set; } = new();
        public List<string> TiposLicenciaDisponibles { get; set; } = new();

        public List<FilaValidacion> Errores { get; set; } = new();
        public List<FilaImportacionActivos> FilasDetectadas { get; set; } = new();
    }

    public class FilaValidacion
    {
        public int NumeroFila { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class FilaImportacionActivos
    {
        public int NumeroFila { get; set; }
        public bool Seleccionado { get; set; } = true;

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