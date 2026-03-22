using DataAccess.Modelos.DTOs.Autenticacion;

namespace BusinessLogic.Servicios.Autenticacion
{
    public interface ILoginAttemptService
    {
        Task RegistrarAsync(string emailIngresado, string? userId, bool exitoso, string? motivoFallo, string? ip, string? userAgent);
        Task<IReadOnlyList<LoginAttemptItemDto>> ObtenerUltimosPorUsuarioAsync(string userId, int take);
        Task<LoginAttemptPagedResultDto> ObtenerIntentosAsync(LoginAttemptFiltroDto filtro);

    }
}