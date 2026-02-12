using BusinessLogic.Servicios.Correo.Plantillas;

namespace BusinessLogic.Servicios.Correo
{
    public class CorreoNotificacionesService : ICorreoNotificacionesService
    {
        private readonly IEmailService _emailService;

        public CorreoNotificacionesService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<bool> EnviarActivacionCuentaAsync(string toEmail, string nombreUsuario, string activationLink)
        {
            var subject = "Activa tu cuenta - Sistema de Gestión SASA";
            var html = PlantillasCorreo.ActivacionCuenta(nombreUsuario, activationLink);

            try
            {
                await _emailService.SendEmailAsync(toEmail, nombreUsuario, subject, html);
                return true;
            }
            catch
            {
                // Regla: si falla NO rompe el sistema
                return false;
            }
        }
    }
}
