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

        public async Task EnviarActivacionCuentaAsync(string toEmail, string nombreUsuario, string activationLink)
        {
            var subject = "Activa tu cuenta - Sistema de Gestión SASA";
            var html = PlantillasCorreo.ActivacionCuenta(nombreUsuario, activationLink);

            try
            {
                await _emailService.SendEmailAsync(toEmail, nombreUsuario, subject, html);
            }
            catch
            {
                // Regla: si falla el correo NO rompe el sistema.
                // (Luego podemos loguear con ILogger desde SASA si quieres.)
            }
        }
    }
}
