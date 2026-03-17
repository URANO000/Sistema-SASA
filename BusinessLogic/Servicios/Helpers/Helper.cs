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

        //Para verificar que un usuario es activo
        public async Task ValidarUsuarioEstado(string currentUserId)
        {
            var user = await _userManager.FindByIdAsync(currentUserId);

            if (user == null)
            {
                throw new KeyNotFoundException("El usuario no existe.");
            }

            if (user.Estado == false)
            {
                throw new UnauthorizedAccessException("Un usuario inactivo no puede realizar ninguna operación.");
            }
        }
    }
}
