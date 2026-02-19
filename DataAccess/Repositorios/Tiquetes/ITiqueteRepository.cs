using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades;
namespace DataAccess.Repositorios.Tiquetes
{
    public interface ITiqueteRepository
    {
        //Métodos para el repositorio de Tiquete - ESP
        Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro); //Listar con filtros
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesReporteAsync(); //Lista sin filtros para reportes
        Task<ListaTiqueteDTO?> ObtenerTiquetePorIdReadAsync(int id); //Detalle -- READONLY
        Task<Tiquete?> ObtenerEntidadPorIdAsync(int id); //Obtener la entidad completa por ID (para edición)
        Task<Tiquete> AgregarTiqueteAsync(Tiquete tiquete); //Crear
        Task ActualizarTiqueteAsync(Tiquete tiquete); //Actualizar
        Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id);
    }
}