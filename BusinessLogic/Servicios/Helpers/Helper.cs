using DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace BusinessLogic.Servicios.Helpers
{
    public class Helper : IHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly TimeZoneInfo ZonaCR = ObtenerZonaCR();
        public Helper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //Para auditoria de usuarios
        private static TimeZoneInfo ObtenerZonaCR()
        {
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Central America Standard Time"
                : "America/Costa_Rica";

            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        public DateTime ObtenerAhoraCR()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ZonaCR);
        }

        public DateOnly ObtenerFechaHoyCR()
        {
            return DateOnly.FromDateTime(ObtenerAhoraCR());
        }

        public TimeSpan ObtenerHoraCR()
        {
            return TimeOnly.FromDateTime(ObtenerAhoraCR()).ToTimeSpan();
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

        public string FormatTiempo(TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return $"{(int)span.TotalDays}d {span.Hours}h {span.Minutes}m";
            }

            return $"{(int)span.TotalHours}h {span.Minutes}m";
        }
    }
}
