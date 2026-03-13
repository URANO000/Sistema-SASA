using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;
namespace DataAccess.Repositorios.Tiquetes
{
    public interface ITiqueteRepository
    {
        //Métodos para el repositorio de Tiquete - ESP
        Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro, string? currentUserId); //Listar con filtros
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesReporteAsync(); //Lista sin filtros para reportes
        Task<DetalleTiqueteDto?> ObtenerTiquetePorIdReadAsync(int id); //Detalle -- READONLY
        Task<Tiquete?> ObtenerEntidadPorIdAsync(int id); //Obtener la entidad completa por ID (para edición)
        Task<Tiquete> AgregarTiqueteAsync(Tiquete tiquete); //Crear
        Task ActualizarTiqueteAsync(Tiquete tiquete); //Actualizar
        Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id);

        Task<bool> ExisteTiquete(int id);
        Task<List<Tiquete>> ObtenerTiquetesPorIdsAsync(List<int> ids);
        Task ActualizarAsignacionAsync(List<Tiquete> tiquetes);
    }
}