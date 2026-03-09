namespace BusinessLogic.Servicios.Correo
{
    public interface ICorreoNotificacionesService
    {
        Task<bool> EnviarActivacionCuentaAsync(string toEmail, string nombreUsuario, string activationLink);
        Task<bool> EnviarRecuperacionContrasenaAsync(string toEmail, string nombreUsuario, string resetLink);
        Task<bool> EnviarTiqueteCreadoAsync(string toEmail, string nombreUsuario, int tiqueteId, string asunto, string detalleLink);
    }
}
