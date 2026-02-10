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

        public async Task NotificarConfirmacionPermisoAsync(string toEmail, string nombreEmpleado, string tipoPermiso)
        {
            var subject = "Confirmación de permiso - Sistema de Gestión SASA";
            var html = PlantillasCorreo.ConfirmacionEmpleado(nombreEmpleado, tipoPermiso);

            try
            {
                await _emailService.SendEmailAsync(toEmail, nombreEmpleado, subject, html);
            }
            catch
            {
                // Regla: si falla correo NO rompe el sistema.
                // (Ideal: log en SASA; pero aquí al menos no explota.)
            }
        }
    }
}