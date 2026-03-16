using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Modelos.DTOs.Wrappers;

namespace BusinessLogic.Servicios.Inventario
{
    public interface IInventarioService
    {
        Task<PagedResult<ActivoTelefonoInventarioListItemDto>> ListarPaginadoAsync(ActivoInventarioFiltroDto filtros);
        Task<ActivoInventarioDetailDto?> ObtenerDetalleAsync(int id);
        Task<(bool ok, string? error)> CrearAsync(ActivoInventarioCreateDto dto);
        Task<(bool ok, string? error)> ActualizarAsync(int id, ActivoInventarioEditDto dto);

        Task<(bool ok, string? error)> AsociarActivoConTiqueteAsync(ActivoTiqueteAsociacionDto dto);
        Task<IReadOnlyList<ActivoTelefonoInventarioListItemDto>> ObtenerActivosParaAsociacionAsync();

        Task<IReadOnlyList<MantenimientoActivoListItemDto>> ObtenerHistorialMantenimientoAsync(int? idActivo = null);
        Task<MantenimientoActivoListItemDto?> ObtenerMantenimientoPorIdAsync(int id);
        Task<(bool ok, string? error)> RegistrarMantenimientoAsync(CrearMantenimientoActivoDto dto);
        Task<(bool ok, string? error)> ActualizarMantenimientoAsync(int id, CrearMantenimientoActivoDto dto);

        Task<IReadOnlyList<ActivoTelefonoInventarioListItemDto>> ObtenerActivosReporteGeneralAsync();
        Task<Dictionary<string, int>> ObtenerResumenPorEstadoAsync();
    }
}