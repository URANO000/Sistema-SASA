namespace DataAccess.Modelos.DTOs.InventarioTelefono
{
    public class ActivoTelefonoFiltroDto
    {
        public string? Texto { get; set; }

        public string? Operador { get; set; }

        public string? Departamento { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "Nombre";

        public string SortDir { get; set; } = "asc";
    }
}