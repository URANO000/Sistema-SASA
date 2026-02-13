using DataAccess.Modelos.DTOs.Notificaciones;
using DataAccess.Repositorios.Notificaciones;

namespace BusinessLogic.Servicios.Notificaciones
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _repo;

        public NotificacionService(INotificacionRepository repo)
        {
            _repo = repo;
        }

        public Task<ResultadoPaginadoDTO<NotificacionDTO>> ObtenerPorUsuarioAsync(string userId, string? q, string? tipo, string? estado, DateTime? fecha, int pagina, int tamanoPagina)
            => _repo.ObtenerPorUsuarioAsync(userId, q, tipo, estado, fecha, pagina, tamanoPagina);

        public Task<NotificacionDTO?> ObtenerPorIdAsync(long idNotificacion, string userId)
            => _repo.ObtenerPorIdAsync(idNotificacion, userId);

        public Task MarcarComoLeidaAsync(long idNotificacion, string userId)
            => _repo.MarcarComoLeidaAsync(idNotificacion, userId);

        public Task AlternarLeidaAsync(long idNotificacion, string userId)
            => _repo.AlternarLeidaAsync(idNotificacion, userId);

        public Task MarcarTodasComoLeidasAsync(string userId)
            => _repo.MarcarTodasComoLeidasAsync(userId);
        public Task<int> ContarNoLeidasAsync(string userId)
            => _repo.ContarNoLeidasAsync(userId);
        public Task NotificarNuevoComentarioAsync(int idTiquete, string autorUserId, string mensaje)
            => _repo.NotificarNuevoComentarioAsync(idTiquete, autorUserId, mensaje);
        public Task<DateTime?> ObtenerSilencioActivoAsync(string userId, int idTiquete)
    => _repo.ObtenerSilencioActivoAsync(userId, idTiquete);

        public Task SilenciarTiqueteAsync(string userId, int idTiquete, int horas)
            => _repo.SilenciarTiqueteAsync(userId, idTiquete, horas);

        public Task ReactivarSilencioAsync(string userId, int idTiquete)
            => _repo.ReactivarSilencioAsync(userId, idTiquete);



    }
}
