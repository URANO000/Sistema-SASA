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
            return await _context.Usuarios
                .AsNoTracking()
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    PrimerApellido = u.PrimerApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado,
                    LastActivityUtc = u.LastActivityUtc
                })
                .ToListAsync();
        }

        public async Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Id == id.ToString())
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    PrimerApellido = u.PrimerApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado,
                    LastActivityUtc = u.LastActivityUtc

                })
                .FirstOrDefaultAsync();
        }

        public async Task AgregarUsuarioAsync(CrearUsuarioDto usuario)
        {
            
        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = false; //bit 0
                await _context.SaveChangesAsync();
            }
        }
    }
}
