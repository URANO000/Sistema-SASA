using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;

namespace BusinessLogic.Servicios.Tiquetes
{
    public interface ITiqueteService
    {
        //Métodos para el servicio de Tiquete
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesAsync();
        Task<ListaTiqueteDTO?> ObtenerPorTiqueteIdAsync(int id);
        Task<int> AgregarTiqueteAsync(CrearTiqueteAdminDto tiquete, string currentUserId);
        Task<Tiquete?> ActualizarTiqueteAsync(int id, Tiquete tiquete);
        Task<bool> CancelarTiquete(int id);
    }
}
