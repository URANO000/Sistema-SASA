namespace BusinessLogic.Servicios.Correo
{
    public interface ICorreoNotificacionesService
    {
        Task<bool> EnviarActivacionCuentaAsync(string toEmail, string nombreUsuario, string activationLink);
    }
}
