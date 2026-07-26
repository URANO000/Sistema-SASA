using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;

namespace SASA.ViewModels.Tiquete.Extras
{
    public class AsignarTiquetesViewModel
    {
        public AsignarTiqueteDto Asignacion { get; set; }

        public TiqueteFiltroDto Filtro { get; set; }
    }
}
