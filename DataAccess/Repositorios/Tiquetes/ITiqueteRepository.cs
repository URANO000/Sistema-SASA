using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;
namespace DataAccess.Repositorios.Tiquetes
{
    public interface ITiqueteRepository
    {
        //Métodos para el repositorio de Tiquete - ESP
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesAsync(); //Listar
        Task<ListaTiqueteDTO?> ObtenerTiquetePorIdReadAsync(int id); //Detalle
        Task<Tiquete?> ObtenerEntidadPorIdAsync(int id); //Obtener la entidad completa por ID (para edición)
        Task<Tiquete> AgregarTiqueteAsync(Tiquete tiquete); //Crear
        Task ActualizarTiqueteAsync(Tiquete tiquete); //Actualizar
        Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id);
    }
}