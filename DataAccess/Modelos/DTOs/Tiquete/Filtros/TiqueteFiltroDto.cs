namespace DataAccess.Modelos.DTOs.Tiquete.Filtros
{
    public class TiqueteFiltroDto
    {
        public string? Search { get; set; }
        public string? Estatus { get; set; }
        public string? Prioridad { get; set; }
        public DateTime? Fecha { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10; //Se puede cambiar si se desea
    }
}
