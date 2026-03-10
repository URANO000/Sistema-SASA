using DataAccess.Modelos.DTOs.Autenticacion;

namespace DataAccess.Repositorios.Autenticacion
{
    public interface ILoginAttemptRepository
    {
        Task AddAsync(string emailIngresado, string? userId, bool exitoso, string? motivoFallo, string? ip, string? userAgent);
        Task<IReadOnlyList<LoginAttemptItemDto>> ObtenerUltimosPorUsuarioAsync(string userId, int take);
    }
}