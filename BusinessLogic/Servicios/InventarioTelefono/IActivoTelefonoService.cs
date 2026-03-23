using DataAccess.Modelos.DTOs.InventarioTelefono;

namespace BusinessLogic.Servicios.InventarioTelefono
{
    public interface IActivoTelefonoService
    {
        Task<(List<ActivoTelefonoListItemDto> Items, int TotalRecords)>
            ListarPaginadoAsync(ActivoTelefonoFiltroDto filtros);

        Task<ActivoTelefonoDetailDto?> ObtenerDetalleAsync(int id);

        Task<(bool Ok, string? Error)> CrearAsync(ActivoTelefonoCreateDto dto);

        Task<(bool Ok, string? Error)> ActualizarAsync(int id, ActivoTelefonoEditDto dto);

        Task<byte[]> ExportarExcelAsync(ActivoTelefonoFiltroDto filtros);
    }
}