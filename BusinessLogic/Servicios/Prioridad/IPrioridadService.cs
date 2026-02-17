using DataAccess.Modelos.DTOs.Prioridad;

namespace BusinessLogic.Servicios.Prioridad
{
    public interface IPrioridadService
    {
        public Task<IEnumerable<ListaPrioridadDto>> ObtenerPrioridadesAsync();
    }
}
