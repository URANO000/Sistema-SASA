using BusinessLogic.Servicios.Correo.Plantillas;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Servicios.Correo
{
    public class CorreoNotificacionesService : ICorreoNotificacionesService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<CorreoNotificacionesService> _logger;

        public CorreoNotificacionesService(IEmailService emailService, ILogger<CorreoNotificacionesService> logger)
        {
            _emailService = emailService;
            _logger = logger;
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
                _logger.LogError("Error al enviar correo de activación a: {Email}", toEmail);
                return false;
            }
        }

        public async Task<bool> EnviarRecuperacionContrasenaAsync(string toEmail, string nombreUsuario, string resetLink)
        {
            var subject = "Restablecer contraseña - Sistema de Gestión SASA";
            var html = PlantillasCorreo.RecuperacionContrasena(nombreUsuario, resetLink);

            try
            {
                await _emailService.SendEmailAsync(toEmail, nombreUsuario, subject, html);
                return true;
            }
            catch
            {
                return false; // regla: no rompe el sistema
            }
        }

        public async Task<bool> EnviarTiqueteCreadoAsync(string toEmail, string nombreUsuario, int tiqueteId, string asunto, string detalleLink)
        {
            var subject = $"Tiquete creado - {asunto}";
            var html = PlantillasCorreo.TiqueteCreado(nombreUsuario, tiqueteId, asunto, detalleLink);

            try
            {
                await _emailService.SendEmailAsync(toEmail, nombreUsuario, subject, html);
                return true;
            }
            catch
            {
                // No propagamos la excepción: la regla del sistema es no romper el flujo por fallos en correo
                return false;
            }
        }

        public async Task<bool> EnviarTiqueteCreadoAdminsAsync(
    IEnumerable<string> admins,
    int tiqueteId,
    string asunto,
    string nombreReportador,
    string correoReportador,
    string detalleLink)
        {
            try
            {
                var destinatarios = admins?
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (destinatarios == null || destinatarios.Count == 0)
                    return false;

                var subject = $"Nuevo tiquete creado #{tiqueteId} - Pendiente de revisión";
                var html = PlantillasCorreo.TiqueteCreadoAdmins(
                    tiqueteId,
                    asunto,
                    nombreReportador,
                    correoReportador,
                    detalleLink);

                foreach (var admin in destinatarios)
                {
                    await _emailService.SendEmailAsync(admin, "Administrador TI", subject, html);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EnviarTiqueteResueltoOCerradoAsync(
            string toEmail,
            string nombreUsuario,
            int tiqueteId,
            string asunto,
            string estado,
            string detalleLink)
        {
            try
            {
                var subject = $"Actualización de tiquete #{tiqueteId}";
                var html = PlantillasCorreo.TiqueteResueltoOCerrado(
                    nombreUsuario,
                    tiqueteId,
                    asunto,
                    estado,
                    detalleLink);

                await _emailService.SendEmailAsync(toEmail, nombreUsuario, subject, html);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EnviarNuevoAvanceTiqueteAsync(
            string toEmail,
            string nombreUsuario,
            int tiqueteId,
            string asunto,
            string textoAvance,
            string detalleLink)
        {
            try
            {
                var subject = $"Nuevo avance en el tiquete #{tiqueteId}";
                var html = PlantillasCorreo.NuevoAvanceTiquete(
                    nombreUsuario,
                    tiqueteId,
                    asunto,
                    textoAvance,
                    detalleLink);

                await _emailService.SendEmailAsync(toEmail, nombreUsuario, subject, html);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
