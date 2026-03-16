namespace DataAccess.Modelos.DTOs.InventarioTelefono
{
    public class ActivoTelefonoListItemDto
    {
        public int IdActivoTelefono { get; set; }

        public string NombreColaborador { get; set; } = "";

        public string? Departamento { get; set; }

        public string? Operador { get; set; }

        public string? NumeroCelular { get; set; }

        public string? Modelo { get; set; }
    }
}