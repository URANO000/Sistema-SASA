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
        public async Task<IEnumerable<Usuario>> ObtenerUsuariosAsync()
            => await _context.Usuarios.ToListAsync();

        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
            => await _context.Usuarios.FindAsync(id);

        public async Task AgregarUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
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
