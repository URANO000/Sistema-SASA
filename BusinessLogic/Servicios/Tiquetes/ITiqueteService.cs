using DataAccess.Modelos.Entidades;

namespace BusinessLogic.Servicios.Tiquetes
{
    public interface ITiqueteService
    {
        //Métodos para el servicio de Tiquete
        Task<IEnumerable<Tiquete>> ObtenerTiquetesAsync();
        Task<Tiquete?> ObtenerPorTiqueteIdAsync(int id);
        Task<Tiquete?> AgregarTiqueteAsync(Tiquete tiquete); //Usar DTO
        Task<Tiquete?> ActualizarTiqueteAsync(int id, Tiquete tiquete); //Usar DTO
        Task<bool> CancelarTiquete(int id);
    }
}
