namespace DataAccess.Modelos.DTOs.Integracion
{
    public class ValidacionImportacionTelefonoDto
    {
        public int HistorialId { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;

        public int TotalFilas { get; set; }
        public int FilasValidas { get; set; }
        public int FilasConError { get; set; }

        public List<FilaValidacionTelefonoDto> Errores { get; set; } = new();
        public List<FilaImportacionTelefonoDto> FilasDetectadas { get; set; } = new();
    }

    public class FilaValidacionTelefonoDto
    {
        public int NumeroFila { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class FilaImportacionTelefonoDto
    {
        public int NumeroFila { get; set; }
        public bool Seleccionado { get; set; } = true;

        public string NombreColaborador { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? Operador { get; set; }
        public string? NumeroCelular { get; set; }
        public string? CorreoSistemasAnaliticos { get; set; }
        public string? Modelo { get; set; }
        public string? IMEI { get; set; }
        public bool Cargador { get; set; }
        public bool Auriculares { get; set; }
    }
}