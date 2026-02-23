using DataAccess.Modelos.Entidades.Inventario;

namespace DataAccess.Repositorios.Inventario
{
    public interface ICatalogosInventarioRepository
    {
        Task<HashSet<string>> ObtenerTiposSetAsync();
        Task<HashSet<string>> ObtenerLicenciasSetAsync();

        Task<List<TipoActivoInventario>> ObtenerTiposAsync();
        Task<List<EstadoActivoInventario>> ObtenerEstadosAsync();
        Task<List<TipoLicenciaInventario>> ObtenerLicenciasAsync();
    }
}