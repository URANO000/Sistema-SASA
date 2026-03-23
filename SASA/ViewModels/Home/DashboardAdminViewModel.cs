using SASA.ViewModels.Tiquete.Extras;

namespace SASA.ViewModels.Home
{
    public class DashboardAdminViewModel
    {
        public int TotalTiquetes { get; set; }
        public int TotalInventario { get; set; }
        public double PromedioResolucion { get; set; }
        public string PromedioResolucionFormateado { get; set; }
        public List<TiquetesPorEstadoViewModel> PorEstado { get; set; }
        public List<TiquetesPorDiaViewModel> Ultimos7Dias { get; set; }
        public List<TiquetesPorEstadoViewModel> TiquetesVencidosPorEstado { get; set; }
    }
}
