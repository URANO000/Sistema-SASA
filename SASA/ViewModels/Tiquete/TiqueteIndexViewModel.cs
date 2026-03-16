using SASA.ViewModels.Tiquete.Extras;
using SASA.ViewModels.Tiquete.Filtro;

namespace SASA.ViewModels.Tiquete
{
    public class TiqueteIndexViewModel : TiqueteFormViewModel
    {
        public List<TiqueteListaViewModel> Tiquetes { get; set; } = new();
        public CrearTiqueteViewModel CrearTiquete { get; set; } = new()
        {
            Asunto = string.Empty,
            Descripcion = string.Empty,
            Categoria = 0,
            IdSubCategoria = 0
        };

        public TiqueteFiltroViewModel Filtro { get; set; } = new();
    }
}
