using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;


namespace DataAccess.Repositorios.Usuarios
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync(); //Listar
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id); //Detalle
        Task ActualizarUsuarioAsync(ApplicationUser usuario); //Actualizar
        Task DesactivarUsuario(string id); //Desactivar. Nunca eliminar
    }
}
