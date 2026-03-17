namespace DataAccess.Modelos.DTOs.Tiquete.Filtros
{
    public class DashboardDto
    {
        public int TotalTiquetes { get; set; }
        public int TotalInventario { get; set; }

        public double PromedioResolucion { get; set; }
        public List<TiquetesPorEstadoDto> PorEstado { get; set; }
        public List<TiquetesPorDiaDto> Ultimos7Dias { get; set; }
    }
}
