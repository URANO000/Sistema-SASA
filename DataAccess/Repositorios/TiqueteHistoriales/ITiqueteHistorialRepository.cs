using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.Entidades;

namespace DataAccess.Repositorios.TiqueteHistoriales
{
    public interface ITiqueteHistorialRepository
    {
        Task<TiqueteHistorial> AgregarTiqueteHistorialAsync(TiqueteHistorial tiquete);
        Task<List<TiqueteHistorialPorIdDto>> GetHistorialByTiqueteIdAsync(int idTiquete);
    }
}
