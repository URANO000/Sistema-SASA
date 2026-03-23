using DataAccess.Identity;

namespace BusinessLogic.Servicios.Helpers
{
    public interface IHelper
    {
        public DateTime FormatearCRTime(DateTime dateTime);
        public void ValidarUsuarioActual(string currentUserId);
        Task ValidarUsuarioEstado(string currentUserId);

        public void ValidarUsuarioExiste(ApplicationUser? user);

        (string? restante, string? excedido, bool atrasado) Calcular(DateTime createdAt, int duracionMinutos);
        string FormatearDuracionDesdeMinutos(int? duracionMinutos);

        string FormatTiempo(TimeSpan span);

        DateTime ObtenerAhoraCR();
        DateOnly ObtenerFechaHoyCR();
        TimeSpan ObtenerHoraCR();
    }
}
