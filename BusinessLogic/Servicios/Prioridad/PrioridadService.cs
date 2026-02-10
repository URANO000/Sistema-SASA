using DataAccess.Modelos.DTOs.Prioridad;
using DataAccess.Repositorios.Prioridad;

namespace BusinessLogic.Servicios.Prioridad
{
    public class PrioridadService : IPrioridadService
    {
        private readonly IPrioridadRepository _repository;
        public PrioridadService(IPrioridadRepository repository)
        {
            _repository = repository;
        }


        //Métodos
        public async Task<IReadOnlyList<ListaPrioridadDto>> ObtenerPrioridadesAsync()
        {
            return await _repository.ObtenerPrioridadesAsync();
        }
    }
}
