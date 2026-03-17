namespace SASA.ViewModels.TiqueteHistoriales
{
    public class TiqueteHistorialIndexViewModel
    {
        public List<ListaTiqueteHistorialViewModel> TiqueteHistorial { get; set; } = new();
        public TiqueteHistorialFiltroViewModel Filtro { get; set; } = new();
    }
}
