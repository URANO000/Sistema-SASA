using DataAccess.Modelos.Entidades;
namespace DataAccess.Repositorios.Tiquetes
{
    public interface ITiqueteRepository
    {
        //Métodos para el repositorio de Tiquete - ESP
        Task<IEnumerable<Tiquete>> ObtenerTiquetesAsync(); //Listar
        Task<Tiquete?> ObtenerTiquetePorIdAsync(int id); //Detalle
        Task AgregarTiqueteAsync(Tiquete tiquete); //Crear
        Task ActualizarTiqueteAsync(Tiquete tiquete); //Actualizar
        Task CancelarTiquete(int id); //Cancelar (no se elimina)
    }
}