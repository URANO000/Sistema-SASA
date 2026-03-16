namespace BusinessLogic.Servicios.Correo
{
    public interface ICorreoNotificacionesService
    {
        Task<bool> EnviarActivacionCuentaAsync(string toEmail, string nombreUsuario, string activationLink);
        Task<bool> EnviarRecuperacionContrasenaAsync(string toEmail, string nombreUsuario, string resetLink);
        Task<bool> EnviarTiqueteCreadoAsync(string toEmail, string nombreUsuario, int tiqueteId, string asunto, string detalleLink);
        Task<bool> EnviarTiqueteCreadoAdminsAsync(IEnumerable<string> admins, int tiqueteId, string asunto, string nombreReportador, string correoReportador, string detalleLink);
        Task<bool> EnviarTiqueteResueltoOCerradoAsync(string toEmail, string nombreUsuario, int tiqueteId, string asunto, string estado, string detalleLink);
        Task<bool> EnviarNuevoAvanceTiqueteAsync(string toEmail, string nombreUsuario, int tiqueteId, string asunto, string textoAvance, string detalleLink);
    }
}
