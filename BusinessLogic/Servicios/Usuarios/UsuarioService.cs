using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.Entidades;
using DataAccess.Repositorios.Usuarios;

namespace BusinessLogic.Servicios.Usuarios

{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;   
        }

        //Implementación de los métodos para el servicio de Usuario

        public async Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync()
        {
            return await _usuarioRepository.ObtenerUsuariosAsync();
        }
        public async Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);
        }
        public Task<Usuario?> ActualizarUsuarioAsync(int id, Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<CrearUsuarioDto?> AgregarUsuarioAsync(CrearUsuarioDto usuario)
        {
            throw new NotImplementedException();
        }

        public Task DesactivarUsuarioAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
