using DataAccess.Modelos.DTOs.Notificaciones;

namespace BusinessLogic.Servicios.Notificaciones
{
    public interface INotificacionService
    {
        Task<ResultadoPaginadoDTO<NotificacionDTO>> ObtenerPorUsuarioAsync(string userId, int pagina, int tamanoPagina);
        Task<NotificacionDTO?> ObtenerPorIdAsync(long idNotificacion, string userId);

        Task MarcarComoLeidaAsync(long idNotificacion, string userId);
        Task AlternarLeidaAsync(long idNotificacion, string userId);
        Task MarcarTodasComoLeidasAsync(string userId);
    }
}
