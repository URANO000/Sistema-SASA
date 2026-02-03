using DataAccess.Modelos.DTOs.Notificaciones;

namespace BusinessLogic.Servicios.Notificaciones
{
    public interface INotificacionService
    {
        Task<ResultadoPaginadoDTO<NotificacionDTO>> ObtenerPorUsuarioAsync(string userId, int pagina, int tamanoPagina);
        Task<NotificacionDTO?> ObtenerPorIdAsync(long idNotificacion, string userId);
        Task NotificarNuevoComentarioAsync(int idTiquete, string autorUserId, string mensaje);
        Task MarcarComoLeidaAsync(long idNotificacion, string userId);
        Task AlternarLeidaAsync(long idNotificacion, string userId);
        Task MarcarTodasComoLeidasAsync(string userId);
        Task<int> ContarNoLeidasAsync(string userId);


    }
}
