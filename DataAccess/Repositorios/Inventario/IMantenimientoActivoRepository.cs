using DataAccess.Modelos.Entidades.Inventario;

namespace DataAccess.Repositorios.Inventario
{
    public interface IMantenimientoActivoRepository
    {
        Task<List<MantenimientoActivo>> ListarAsync();
        Task<List<MantenimientoActivo>> ListarPorActivoAsync(int idActivo);
        Task<MantenimientoActivo?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteMantenimientoEnProcesoAsync(int idActivo, int? excluirIdMantenimiento = null);
        Task CrearAsync(MantenimientoActivo entity);
        Task ActualizarAsync(MantenimientoActivo entity);
        Task GuardarAsync();
    }
}
