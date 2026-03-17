namespace BusinessLogic.Servicios.Helpers
{
    public interface IHelper
    {
        public DateTime FormatearCRTime(DateTime dateTime);
        public void ValidarUsuarioActual(string currentUserId);
        Task ValidarUsuarioEstado(string currentUserId);

        (string? restante, string? excedido, bool atrasado) Calcular(DateTime createdAt, int duracionMinutos);
    }
}
