using System;
using System.Collections.Generic;

namespace DataAccess.Modelos.DTOs.Notificaciones
{
    public class ResultadoPaginadoDTO<T>
    {
        public List<T> Elementos { get; set; } = new();
        public int Pagina { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalRegistros { get; set; }

        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
        public bool TieneAnterior => Pagina > 1;
        public bool TieneSiguiente => Pagina < TotalPaginas;
    }
}
