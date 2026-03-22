using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.DTOs.TiqueteHistorial.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;

namespace DataAccess.Repositorios.TiqueteHistoriales
{
    public interface ITiqueteHistorialRepository
    {
        Task<TiqueteHistorial> AgregarTiqueteHistorialAsync(TiqueteHistorial tiquete, bool autoSave);
        Task<List<TiqueteHistorialPorIdDto>> GetHistorialByTiqueteIdAsync(int idTiquete);
        Task<PagedResult<ListaTiqueteHistorialDto>> ListarHistorialAsync(TiqueteHistorialFiltroDto filtro);
    }
}
