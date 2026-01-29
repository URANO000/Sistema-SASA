using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;
using DataAccess.Repositorios.Tiquetes;

namespace BusinessLogic.Servicios.Tiquetes
{
    public class TiqueteService : ITiqueteService
    {
        //Repositorio de Tiquete
        private readonly ITiqueteRepository _tiqueteRepository;

        public TiqueteService(ITiqueteRepository tiqueteRepository)
        {
            _tiqueteRepository = tiqueteRepository;   
        }
        //Implementación de los métodos para el servicio de Tiquete
        public async Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesAsync()
        {
            return await _tiqueteRepository.ObtenerTiquetesAsync();
        }
        public async Task<ListaTiqueteDTO?> ObtenerPorTiqueteIdAsync(int id)
        {
            return await _tiqueteRepository.ObtenerTiquetePorIdAsync(id);
        }

        public Task<Tiquete?> AgregarTiqueteAsync(Tiquete tiquete)
        {
            throw new NotImplementedException();
        }

        public Task<Tiquete?> ActualizarTiqueteAsync(int id, Tiquete tiquete)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelarTiquete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
