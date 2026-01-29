using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.Entidades;


namespace DataAccess.Repositorios.Usuarios
{
    public interface IUsuarioRepository
    {
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync(); //Listar
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(int id); //Detalle
        Task AgregarUsuarioAsync(CrearUsuarioDto usuario); //Agregar
        Task ActualizarUsuarioAsync(Usuario usuario); //Actualizar
        Task DesactivarUsuario(int id); //Desactivar. Nunca eliminar
    }
}
