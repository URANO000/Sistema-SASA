namespace DataAccess.Modelos.DTOs.TiqueteHistorial.Filtros
{
    public class TiqueteHistorialFiltroDto
    {
        public string? Search { get; set; }

        //Filtros
        public string? TipoEvento { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFinal { get; set; }
        public DateTime? Fecha { get; set; }


        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10; //Se puede cambiar si se desea

    }
}
