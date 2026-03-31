using DataAccess.Modelos.DTOs.Integracion;
using DataAccess.Modelos.Entidades.Integracion;

namespace BusinessLogic.Servicios.Integracion
{
    public interface IIntegracionService
    {
        Task<int> RegistrarCargaAsync(string nombreArchivoOriginal, string rutaArchivoRelativa, string? usuarioId, string tipoCarga);

        // Equipos
        Task<ValidacionImportacionDto> ValidarInventarioDesdeExcelAsync(int historialId, string webRootPath);
        Task<(bool Ok, string Mensaje, ValidacionImportacionDto Resultado)> ConfirmarImportacionInventarioEditadoAsync(
            int historialId,
            List<FilaImportacionActivosDto> filas,
            string? usuarioId);

        // Teléfonos
        Task<ValidacionImportacionTelefonoDto> ValidarTelefonosDesdeExcelAsync(int historialId, string webRootPath);
        Task<(bool Ok, string Mensaje, ValidacionImportacionTelefonoDto Resultado)> ConfirmarImportacionTelefonosAsync(
            int historialId,
            List<FilaImportacionTelefonoDto> filas,
            string? usuarioId);

        Task<List<IntegracionHistorial>> ObtenerHistorialAsync();

        Task<(bool Ok, string Mensaje)> DeshabilitarArchivoAsync(int historialId, string? usuarioId);
        Task<(bool Ok, string Mensaje)> ReprocesarArchivoAsync(int historialId, string webRootPath, string? usuarioId);
    }
}