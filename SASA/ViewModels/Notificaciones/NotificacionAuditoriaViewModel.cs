using System.Collections.Generic;

namespace SASA.ViewModels.Notificaciones
{
    public class NotificacionAuditoriaViewModel
    {

        public string? Q { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public DateTime? Fecha { get; set; }

     
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 10;
        public int TotalRegistros { get; set; }

        public List<NotificacionAuditoriaItemViewModel> Elementos { get; set; } = new();
    }
}
