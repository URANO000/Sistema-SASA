using DataAccess.Modelos.Entidades.Inventario;

namespace DataAccess.Repositorios.Inventario
{
    public interface IActivoInventarioRepository
    {
        Task<List<string>> ObtenerNumerosExistentesAsync(IEnumerable<string> numeros);
        Task AgregarRangoAsync(IEnumerable<ActivoInventario> activos);
        Task<bool> ExisteNumeroAsync(string numeroActivo);
        Task GuardarAsync();
    }
}