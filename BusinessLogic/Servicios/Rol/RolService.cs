using DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace BusinessLogic.Servicios.Rol
{
    public class RolService : IRolService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        //Implementación del método para el servicio de Rol
        public async Task<IReadOnlyList<string>> ObtenerRolesAsync()
        {
            return await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();
        }
    }
}
