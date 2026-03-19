using DataAccess.Identity;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Servicios.Helpers
{
    public class Helper : IHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public Helper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string FormatearDuracionDesdeMinutos(int? duracionMinutos)
        {
            if (!duracionMinutos.HasValue)
                return "—";

            var span = TimeSpan.FromMinutes(duracionMinutos.Value);
            // Weeks (7-day periods)
            if (span.TotalDays >= 7)
            {
                var weeks = span.Days / 7;
                var daysRem = span.Days % 7;

                var weeksPart = weeks == 1 ? "1 semana" : $"{weeks} semanas";

                if (daysRem > 0)
                {
                    var daysPart = daysRem == 1 ? "1 día" : $"{daysRem} días";
                    return $"{weeksPart} {daysPart}";
                }

                return weeksPart;
            }

            // Days and hours
            if (span.TotalDays >= 1)
            {
                var days = span.Days;
                var hours = span.Hours;

                var daysPart = days == 1 ? "1 día" : $"{days} días";
                if (hours > 0)
                {
                    var hoursPart = hours == 1 ? "1 hora" : $"{hours} horas";
                    return $"{daysPart} {hoursPart}";
                }

                return daysPart;
            }

            // Hours (omit minutes)
            if (span.TotalHours >= 1)
            {
                var hours = (int)span.TotalHours;
                return hours == 1 ? "1 hora" : $"{hours} horas";
            }

            // Less than 1 hour -> show minutes
            var minutes = span.Minutes;
            return minutes == 1 ? "1 minuto" : $"{minutes} minutos";
        }

        //Para la fecha UTC a CR time para los listados, etc
        public DateTime FormatearCRTime(DateTime dateTime)
        {
            var CRTime = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            var newTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, CRTime);

            return newTime;
        }


        //Para verificar que un usuario está autenticado
        public void ValidarUsuarioActual(string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("Usuario no autenticado.");
        }

        //Para verificar que un usuario existe
        public void ValidarUsuarioExiste(ApplicationUser? user)
        {
            if (user == null)
            {
                throw new KeyNotFoundException("El usuario no existe.");
            }
        }

        //Para verificar que un usuario es activo
        public async Task ValidarUsuarioEstado(string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(currentUserId);

            ValidarUsuarioExiste(user);

            if (user.Estado == false)
            {
                throw new UnauthorizedAccessException("Un usuario inactivo no puede realizar ninguna operación.");
            }
        }

        //Para formatear prioridades
        public (string? restante, string? excedido, bool atrasado) Calcular(
               DateTime createdAt,
               int duracionMinutos)
        {
            var elapsed = DateTime.UtcNow - createdAt;
            var sla = TimeSpan.FromMinutes(duracionMinutos);
            var remaining = sla - elapsed;

            if (remaining > TimeSpan.Zero)
            {
                return (
                    FormatTiempo(remaining),
                    null,
                    false
                );
            }

            var overdue = remaining.Duration();

            return (
                null,
                FormatTiempo(overdue),
                true
            );
        }

        private string FormatTiempo(TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return $"{(int)span.TotalDays}d {span.Hours}h {span.Minutes}m";
            }

            return $"{(int)span.TotalHours}h {span.Minutes}m";
        }
    }
}
