using DataAccess.Modelos.Entidades.Inventario;

namespace DataAccess.Repositorios.Inventario
{
    public interface IActivoInventarioRepository
    {
        // Lecturas
        Task<ActivoInventario?> ObtenerPorIdAsync(int id);
        Task<ActivoInventario?> ObtenerDetalleAsync(int id); // includes (si aplica)
        Task<List<ActivoInventario>> ListarAsync(string? q, int? estadoId, int? tipoId);

        // Validaciones / soporte Integración
        Task<bool> ExisteNumeroActivoAsync(string numeroActivo);
        Task<List<string>> ObtenerNumerosExistentesAsync(IEnumerable<string> numeros);

        // Escrituras
        Task CrearAsync(ActivoInventario entity);
        Task ActualizarAsync(ActivoInventario entity);

        // Soporte Integración
        Task AgregarRangoAsync(IEnumerable<ActivoInventario> entities);

        // Unit of Work
        Task GuardarAsync();
    }
}