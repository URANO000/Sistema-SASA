using DataAccess.Modelos.DTOs.Prioridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositorios.Prioridad
{
    public interface IPrioridadRepository
    {
        public Task<IReadOnlyList<ListaPrioridadDto>> ObtenerPrioridadesAsync();
    }
}
