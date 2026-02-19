using SASA.ViewModels.Tiquete.Extras;
using SASA.ViewModels.Tiquete.Filtro;

namespace SASA.ViewModels.Tiquete
{
    public class TiqueteIndexViewModel
    {
        public List<TiqueteListaViewModel> Tiquetes { get; set; } = new();
        public TiqueteFiltroViewModel Filtro { get; set; } = new();
    }
}
