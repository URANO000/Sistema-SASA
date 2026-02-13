using System;
using System.Collections.Generic;
using DataAccess.Modelos.DTOs.Notificaciones;

namespace SASA.ViewModels.Notificaciones
{
    public class NotificacionIndexViewModel
    {
        public string? Q { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public DateTime? Fecha { get; set; }

        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 10;
        public int TotalRegistros { get; set; }

        public List<NotificacionDTO> Elementos { get; set; } = new();

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
        public bool TieneAnterior => Pagina > 1;
        public bool TieneSiguiente => Pagina < TotalPaginas;
    }
}
