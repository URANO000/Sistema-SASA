using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Usuarios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        //Referencia al contexto de la base de datos
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Implementación de los métodos del repositorio de usuarios
        public async Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado
                })
                .ToListAsync();
        }

        public async Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado
                })
                .FirstOrDefaultAsync();
        }

        public async Task ActualizarUsuarioAsync(ApplicationUser usuario)
        {
            _context.Users.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarUsuario(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = false; //bit 0
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivarUsuario(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = true; //bit 1
                await _context.SaveChangesAsync();
            }
        }

        //Obtener usuarios de TI
        public async Task<IReadOnlyList<UsuarioTIDropdownDto?>> ObtenerUsuariosTIAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Departamento == "Tecnologías de Información")
                .OrderBy(u => u.PrimerNombre)
                .Select(u => new UsuarioTIDropdownDto
                {
                    Id = u.Id,
                    UserName = u.UserName
                })
                .ToListAsync();
        }

    }
}
