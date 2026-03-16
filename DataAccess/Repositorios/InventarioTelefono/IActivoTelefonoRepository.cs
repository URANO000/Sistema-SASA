using DataAccess.Modelos.Entidades.InventarioTelefono;

namespace DataAccess.Repositorios.InventarioTelefono
{
    public interface IActivoTelefonoRepository
    {
        Task<ActivoTelefono?> ObtenerPorIdAsync(int id);

        Task<int> ContarAsync(string? texto);

        Task<List<ActivoTelefono>> ListarPaginadoAsync(
            string? texto,
            int skip,
            int take,
            string sortBy,
            string sortDir);

        Task<bool> ExisteImeiAsync(string imei);

        Task<bool> ExisteImeiAsync(string imei, int idExcluir);

        Task CrearAsync(ActivoTelefono entity);

        Task ActualizarAsync(ActivoTelefono entity);

        Task GuardarAsync();
    }
}