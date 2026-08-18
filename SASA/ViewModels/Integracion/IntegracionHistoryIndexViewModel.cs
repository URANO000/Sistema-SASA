using System.Collections.Generic;
using DataAccess.Modelos.Entidades.Integracion;

namespace SASA.ViewModels.Integracion
{
    public class IntegracionHistoryIndexViewModel
    {
        public List<IntegracionHistorial> Historial { get; set; } = new List<IntegracionHistorial>();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;

        public bool TieneAnterior => PageNumber > 1;
        public bool TieneSiguiente => PageNumber < TotalPages;
    }
}
