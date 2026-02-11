namespace BusinessLogic.Servicios.Correo
{
    public interface ICorreoNotificacionesService
    {
        Task EnviarActivacionCuentaAsync(string toEmail, string nombreUsuario, string activationLink);
    }
}
