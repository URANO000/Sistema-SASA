using DataAccess.Modelos.Entidades.Inventario;

namespace DataAccess.Repositorios.Inventario
{
    public interface IActivoInventarioTiqueteRepository
    {
        Task<bool> ExisteAsociacionAsync(int idActivo, int idTiquete);

        Task CrearAsync(ActivoInventarioTiquete entity);

        Task GuardarAsync();
    }
}