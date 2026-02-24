using DataAccess.Modelos.DTOs.Integracion;
using DataAccess.Modelos.Entidades.Integracion;

namespace BusinessLogic.Servicios.Integracion
{
    public interface IIntegracionService
    {
        Task<int> RegistrarCargaAsync(string nombreArchivoOriginal, string rutaArchivoRelativa, string? usuarioId);
        Task<ValidacionImportacionDto> ValidarInventarioDesdeExcelAsync(int historialId, string webRootPath);
        Task ConfirmarImportacionInventarioAsync(int historialId, string webRootPath, string? usuarioId);
        Task<List<IntegracionHistorial>> ObtenerHistorialAsync();
    }
}