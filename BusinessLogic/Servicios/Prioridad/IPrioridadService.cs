using DataAccess.Modelos.DTOs.Prioridad;

namespace BusinessLogic.Servicios.Prioridad
{
    public interface IPrioridadService
    {
        public Task<IReadOnlyList<ListaPrioridadDto>> ObtenerPrioridadesAsync();
    }
}
