namespace DataAccess.Modelos.DTOs.Usuarios.Filtros
{
    public class UsuarioFiltroDto
    {
        public string? Search { get; set; }
        public string? Departamento { get; set; }
        public bool? Estado { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
