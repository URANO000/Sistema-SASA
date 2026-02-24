using BusinessLogic.Modelos.DTOs.Inventario;
using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Modelos.DTOs.Wrappers;

namespace BusinessLogic.Servicios.Inventario
{
    public interface IInventarioService
    {
        Task<List<ActivoInventarioListItemDto>> ListarAsync(ActivoInventarioFiltroDto filtros);
        Task<PagedResult<ActivoInventarioListItemDto>> ListarPaginadoAsync(ActivoInventarioFiltroDto filtros);

        Task<ActivoInventarioDetailDto?> ObtenerDetalleAsync(int id);

        Task<(bool ok, string? error)> CrearAsync(ActivoInventarioCreateDto dto);
        Task<(bool ok, string? error)> ActualizarAsync(int id, ActivoInventarioEditDto dto);
    }
}