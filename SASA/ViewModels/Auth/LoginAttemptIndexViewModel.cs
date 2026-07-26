using DataAccess.Modelos.DTOs.Autenticacion;

namespace SASA.ViewModels.Auth
{
    public class LoginAttemptIndexViewModel
    {
        public LoginAttemptFiltroViewModel Filtros { get; set; } = new();
        public IReadOnlyList<LoginAttemptItemDto> Items { get; set; } = new List<LoginAttemptItemDto>();
        public int TotalCount { get; set; }

        public int TotalPages
        {
            get
            {
                if (Filtros.PageSize <= 0 || TotalCount == 0) return 1;
                return (int)Math.Ceiling((double)TotalCount / Filtros.PageSize);
            }
        }

        public bool TieneAnterior => Filtros.Page > 1;
        public bool TieneSiguiente => Filtros.Page < TotalPages;
    }
}
