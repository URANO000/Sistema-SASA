namespace DataAccess.Modelos.DTOs.InventarioTelefono
{
    public class ActivoTelefonoEditDto
    {
        public string NombreColaborador { get; set; } = "";

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