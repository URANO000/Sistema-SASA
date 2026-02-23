using DataAccess.Modelos.Entidades.Integracion;

namespace DataAccess.Repositorios.Integracion
{
    public interface IIntegracionHistorialRepository
    {
        Task<IntegracionHistorial?> ObtenerPorIdAsync(int id);
        Task<int> CrearAsync(IntegracionHistorial hist);
        Task ActualizarAsync(IntegracionHistorial hist);
        Task<List<IntegracionHistorial>> ListarAsync();
        Task GuardarAsync();
    }
}