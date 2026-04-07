using DataAccess.Modelos.DTOs.Integracion;
using DataAccess.Modelos.Entidades.Integracion;

namespace BusinessLogic.Servicios.Integracion
{
    public interface IIntegracionService
    {
        Task<int> RegistrarCargaAsync(string nombreArchivoOriginal, byte[] archivoBytes, string tipoMime, long pesoBytes, string? usuarioId, string tipoCarga);

        // Equipos
        Task<ValidacionImportacionDto> ValidarInventarioDesdeExcelAsync(int historialId);
        Task<(bool Ok, string Mensaje, ValidacionImportacionDto Resultado)> ConfirmarImportacionInventarioEditadoAsync(
            int historialId,
            List<FilaImportacionActivosDto> filas,
            string? usuarioId);

        // Teléfonos
        Task<ValidacionImportacionTelefonoDto> ValidarTelefonosDesdeExcelAsync(int historialId);
        Task<(bool Ok, string Mensaje, ValidacionImportacionTelefonoDto Resultado)> ConfirmarImportacionTelefonosAsync(
            int historialId,
            List<FilaImportacionTelefonoDto> filas,
            string? usuarioId);

        Task<List<IntegracionHistorial>> ObtenerHistorialAsync();

        Task<(bool Ok, string Mensaje)> DeshabilitarArchivoAsync(int historialId, string? usuarioId);
        Task<(bool Ok, string Mensaje)> ReprocesarArchivoAsync(int historialId, string? usuarioId);
    }
}