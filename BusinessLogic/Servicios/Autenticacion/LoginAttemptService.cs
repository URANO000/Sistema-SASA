using DataAccess.Modelos.DTOs.Autenticacion;
using DataAccess.Repositorios.Autenticacion;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Servicios.Autenticacion
{
    public class LoginAttemptService : ILoginAttemptService
    {
        private readonly ILoginAttemptRepository _repo;
        private readonly ILogger<LoginAttemptService> _logger;

        public LoginAttemptService(ILoginAttemptRepository repo, ILogger<LoginAttemptService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task RegistrarAsync(string emailIngresado, string? userId, bool exitoso, string? motivoFallo, string? ip, string? userAgent)
        {
            try
            {
                // best-effort: si falla, NO romper login
                await _repo.AddAsync(emailIngresado, userId, exitoso, motivoFallo, ip, userAgent);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo registrar IntentoInicioSesion (best-effort).");
            }
        }
        public async Task<LoginAttemptPagedResultDto> ObtenerIntentosAsync(LoginAttemptFiltroDto filtro)
        {
            return await _repo.ObtenerIntentosAsync(filtro);
        }


        public Task<IReadOnlyList<LoginAttemptItemDto>> ObtenerUltimosPorUsuarioAsync(string userId, int take)
            => _repo.ObtenerUltimosPorUsuarioAsync(userId, take);
    }
}