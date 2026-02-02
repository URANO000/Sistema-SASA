using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;
namespace DataAccess.Repositorios.Tiquetes
{
    public interface ITiqueteRepository
    {
        //Métodos para el repositorio de Tiquete - ESP
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesAsync(); //Listar
        Task<ListaTiqueteDTO?> ObtenerTiquetePorIdAsync(int id); //Detalle
        Task<Tiquete> AgregarTiqueteAsync(Tiquete tiquete); //Crear
        Task ActualizarTiqueteAsync(Tiquete tiquete); //Actualizar
        Task CancelarTiquete(int id); //Cancelar (no se elimina)
    }
}