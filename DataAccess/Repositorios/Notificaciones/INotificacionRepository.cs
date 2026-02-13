using DataAccess.Modelos.DTOs.Notificaciones;

namespace DataAccess.Repositorios.Notificaciones
{
    public interface INotificacionRepository
    {
        Task<ResultadoPaginadoDTO<NotificacionDTO>> ObtenerPorUsuarioAsync(string userId, int pagina, int tamanoPagina);
        Task<NotificacionDTO?> ObtenerPorIdAsync(long idNotificacion, string userId);
        Task MarcarComoLeidaAsync(long idNotificacion, string userId);
        Task AlternarLeidaAsync(long idNotificacion, string userId);
        Task MarcarTodasComoLeidasAsync(string userId);
        Task<int> ContarNoLeidasAsync(string userId);
        Task NotificarNuevoComentarioAsync(int idTiquete, string autorUserId, string mensaje);
        Task<DateTime?> ObtenerSilencioActivoAsync(string userId, int idTiquete);
        Task SilenciarTiqueteAsync(string userId, int idTiquete, int horas);
        Task ReactivarSilencioAsync(string userId, int idTiquete);

    }
}
