using DataAccess.Modelos.Entidades.Inventario;

namespace DataAccess.Repositorios.Inventario
{
    public interface IInventarioAutomatizadoRepository
    {
        Task<InventarioAutomatizado?> ObtenerExistenteAsync(
            string? serialPC,
            string nombreEquipo);

        Task AgregarAsync(InventarioAutomatizado inventarioAutomatizado);

        Task GuardarAsync();
    }
}