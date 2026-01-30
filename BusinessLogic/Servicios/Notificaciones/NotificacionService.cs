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

        public Task<ResultadoPaginadoDTO<NotificacionDTO>> ObtenerPorUsuarioAsync(string userId, int pagina, int tamanoPagina)
            => _repo.ObtenerPorUsuarioAsync(userId, pagina, tamanoPagina);

        public Task<NotificacionDTO?> ObtenerPorIdAsync(long idNotificacion, string userId)
            => _repo.ObtenerPorIdAsync(idNotificacion, userId);

        public Task MarcarComoLeidaAsync(long idNotificacion, string userId)
            => _repo.MarcarComoLeidaAsync(idNotificacion, userId);

        public Task AlternarLeidaAsync(long idNotificacion, string userId)
            => _repo.AlternarLeidaAsync(idNotificacion, userId);

        public Task MarcarTodasComoLeidasAsync(string userId)
            => _repo.MarcarTodasComoLeidasAsync(userId);
    }
}
