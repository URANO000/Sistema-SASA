using DataAccess.Modelos.DTOs.Notificaciones;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Notificaciones
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly ApplicationDbContext _db;

        public NotificacionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ResultadoPaginadoDTO<NotificacionDTO>> ObtenerPorUsuarioAsync(string userId, int pagina, int tamanoPagina)
        {
            if (pagina < 1) pagina = 1;
            if (tamanoPagina < 1) tamanoPagina = 10;

            var query = _db.Notificaciones
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.FechaCreacion);

            var total = await query.CountAsync();

            var elementos = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(n => new NotificacionDTO
                {
                    IdNotificacion = n.IdNotificacion,
                    IdTiquete = n.IdTiquete,
                    TipoEvento = n.TipoEvento,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    FechaCreacion = n.FechaCreacion
                })
                .ToListAsync();

            return new ResultadoPaginadoDTO<NotificacionDTO>
            {
                Elementos = elementos,
                Pagina = pagina,
                TamanoPagina = tamanoPagina,
                TotalRegistros = total
            };
        }

        public async Task<NotificacionDTO?> ObtenerPorIdAsync(long idNotificacion, string userId)
        {
            return await _db.Notificaciones
                .AsNoTracking()
                .Where(n => n.IdNotificacion == idNotificacion && n.UserId == userId)
                .Select(n => new NotificacionDTO
                {
                    IdNotificacion = n.IdNotificacion,
                    IdTiquete = n.IdTiquete,
                    TipoEvento = n.TipoEvento,
                    Mensaje = n.Mensaje,
                    Leida = n.Leida,
                    FechaCreacion = n.FechaCreacion
                })
                .FirstOrDefaultAsync();
        }

        public async Task MarcarComoLeidaAsync(long idNotificacion, string userId)
        {
            var n = await _db.Notificaciones
                .FirstOrDefaultAsync(x => x.IdNotificacion == idNotificacion && x.UserId == userId);

            if (n == null) return;

            if (!n.Leida)
            {
                n.Leida = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task AlternarLeidaAsync(long idNotificacion, string userId)
        {
            var n = await _db.Notificaciones
                .FirstOrDefaultAsync(x => x.IdNotificacion == idNotificacion && x.UserId == userId);

            if (n == null) return;

            n.Leida = !n.Leida;
            await _db.SaveChangesAsync();
        }

        public async Task MarcarTodasComoLeidasAsync(string userId)
        {
            var notifs = await _db.Notificaciones
                .Where(n => n.UserId == userId && !n.Leida)
                .ToListAsync();

            if (notifs.Count == 0) return;

            foreach (var n in notifs) n.Leida = true;
            await _db.SaveChangesAsync();
        }
        public async Task<int> ContarNoLeidasAsync(string userId)
        {
            return await _db.Notificaciones
                .CountAsync(n => n.UserId == userId && !n.Leida);
        }

    }
}
