using DataAccess.Modelos.DTOs.Autenticacion;
using DataAccess.Modelos.Entidades.Autenticacion;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Autenticacion
{
    public class LoginAttemptRepository : ILoginAttemptRepository
    {
        private readonly ApplicationDbContext _db;

        public LoginAttemptRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(string emailIngresado, string? userId, bool exitoso, string? motivoFallo, string? ip, string? userAgent)
        {
            var entity = new IntentoInicioSesion
            {
                FechaUtc = DateTime.UtcNow,
                EmailIngresado = emailIngresado,
                UserId = userId,
                Exitoso = exitoso,
                MotivoFallo = motivoFallo,
                IpAddress = ip,
                UserAgent = userAgent
            };

            _db.IntentosInicioSesion.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<LoginAttemptItemDto>> ObtenerUltimosPorUsuarioAsync(string userId, int take)
        {
            return await _db.IntentosInicioSesion
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.FechaUtc)
                .Take(take)
                .Select(x => new LoginAttemptItemDto
                {
                    Id = x.Id,
                    FechaUtc = x.FechaUtc,
                    EmailIngresado = x.EmailIngresado,
                    Exitoso = x.Exitoso,
                    MotivoFallo = x.MotivoFallo,
                    IpAddress = x.IpAddress,
                    UserAgent = x.UserAgent
                })
                .ToListAsync();
        }
    }
}