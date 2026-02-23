using DataAccess.Modelos.DTOs.Avances;

namespace BusinessLogic.Servicios.Avances
{
    public interface IAvanceService
    {
        //Métodos para el servicio de avances
        Task<int> AgregarAvanceAsync(CrearAvanceDto dto, string currentUserId, int tiqueteId);
        Task<List<ListaAvancesDto>> ListaAvancesPorTiqueteAsync(int tiqueteId);
    }
}
