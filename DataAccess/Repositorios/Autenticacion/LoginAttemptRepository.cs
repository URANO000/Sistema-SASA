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
        public async Task<LoginAttemptPagedResultDto> ObtenerIntentosAsync(LoginAttemptFiltroDto filtro)
        {
            filtro ??= new LoginAttemptFiltroDto();

            var page = filtro.Page <= 0 ? 1 : filtro.Page;
            var pageSize = filtro.PageSize <= 0 ? 15 : filtro.PageSize;

            var query = _db.IntentosInicioSesion
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.EmailIngresado))
            {
                var emailFiltro = filtro.EmailIngresado.Trim();
                query = query.Where(x => x.EmailIngresado.Contains(emailFiltro));
            }

            if (filtro.Exitoso.HasValue)
            {
                query = query.Where(x => x.Exitoso == filtro.Exitoso.Value);
            }

            if (filtro.FechaDesde.HasValue)
            {
                var fechaDesdeUtc = filtro.FechaDesde.Value.Date;
                query = query.Where(x => x.FechaUtc >= fechaDesdeUtc);
            }

            if (filtro.FechaHasta.HasValue)
            {
                var fechaHastaUtcExclusiva = filtro.FechaHasta.Value.Date.AddDays(1);
                query = query.Where(x => x.FechaUtc < fechaHastaUtcExclusiva);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.FechaUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new LoginAttemptItemDto
                {
                    Id = x.Id,
                    FechaUtc = x.FechaUtc,
                    EmailIngresado = x.EmailIngresado,
                    UserId = x.UserId,
                    Exitoso = x.Exitoso,
                    MotivoFallo = x.MotivoFallo,
                    IpAddress = x.IpAddress,
                    UserAgent = x.UserAgent
                })
                .ToListAsync();

            return new LoginAttemptPagedResultDto
            {
                Items = items,
                TotalCount = totalCount
            };
        }

    }
}