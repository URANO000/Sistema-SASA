using DataAccess.Modelos.DTOs.Notificaciones;
using DataAccess.Modelos.Entidades;
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

        public async Task NotificarNuevoComentarioAsync(int idTiquete, string autorUserId, string mensaje)
        {
            var t = await _db.Tiquetes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTiquete == idTiquete);

            if (t == null) return;

            var responsableId = t.IdAsignee;

            if (string.IsNullOrWhiteSpace(responsableId)) return;

            if (responsableId == autorUserId) return;

            var ahora = DateTime.Now;

            var estaSilenciado = await _db.NotificacionSilencios
                .AsNoTracking()
                .AnyAsync(s =>
                    s.UserId == responsableId &&
                    s.idTiquete == idTiquete &&
                    (s.fechaFin == null || s.fechaFin > ahora)
                );

            if (estaSilenciado) return;

            _db.Notificaciones.Add(new Notificacion
            {
                UserId = responsableId,
                IdTiquete = idTiquete,
                TipoEvento = "NuevoComentario",
                Mensaje = mensaje,
                Leida = false,
                FechaCreacion = ahora
            });

            _db.Auditorias.Add(new Auditoria
            {
                Fecha = DateOnly.FromDateTime(ahora),
                Hora = ahora.TimeOfDay,
                Usuario = autorUserId,
                Tabla = "Notificaciones",
                Accion = $"Emitida NuevoComentario -> Tiquete {idTiquete} -> Destino {responsableId}"
            });

            await _db.SaveChangesAsync();
        }

        public async Task<DateTime?> ObtenerSilencioActivoAsync(string userId, int idTiquete)
        {
            var ahora = DateTime.Now;

            return await _db.NotificacionSilencios
                .AsNoTracking()
                .Where(s => s.UserId == userId
                            && s.idTiquete == idTiquete
                            && (s.fechaFin == null || s.fechaFin > ahora))
                .OrderByDescending(s => s.fechaInicio)
                .Select(s => s.fechaFin)
                .FirstOrDefaultAsync();
        }

        public async Task SilenciarTiqueteAsync(string userId, int idTiquete, int horas)
        {
            if (horas <= 0) horas = 1;

            var ahora = DateTime.Now;
            var hasta = ahora.AddHours(horas);

            var activos = await _db.NotificacionSilencios
                .Where(s => s.UserId == userId
                            && s.idTiquete == idTiquete
                            && (s.fechaFin == null || s.fechaFin > ahora))
                .ToListAsync();

            if (activos.Count > 0)
                _db.NotificacionSilencios.RemoveRange(activos);

            _db.NotificacionSilencios.Add(new NotificacionSilencio
            {
                UserId = userId,
                idTiquete = idTiquete,
                fechaInicio = ahora,
                fechaFin = hasta
            });

            await _db.SaveChangesAsync();
        }

        public async Task ReactivarSilencioAsync(string userId, int idTiquete)
        {
            var ahora = DateTime.Now;

            var activos = await _db.NotificacionSilencios
                .Where(s => s.UserId == userId
                            && s.idTiquete == idTiquete
                            && (s.fechaFin == null || s.fechaFin > ahora))
                .ToListAsync();

            if (activos.Count == 0) return;

            _db.NotificacionSilencios.RemoveRange(activos);
            await _db.SaveChangesAsync();
        }

    }
}
