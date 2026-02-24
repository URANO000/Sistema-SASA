using DataAccess.Modelos.DTOs.Avances;
using DataAccess.Modelos.Entidades.ModTiquete;

namespace DataAccess.Repositorios.Avances
{
    public interface IAvanceRepository
    {
        //Sólamente se necesita agregar avances (por tiquetes), y ver dichos avances por tiquete
        Task<Avance> AgregarAvanceAsync(Avance avance);
        Task<List<ListaAvancesDto>> ListaAvancesPorTiqueteAsync(int idTiquete);
    }
}
