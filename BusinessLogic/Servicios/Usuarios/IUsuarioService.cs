using DataAccess.Modelos.Entidades;

namespace BusinessLogic.Servicios.Usuarios
{
    public interface IUsuarioService
    {
        //Métodos para el servicio de Usuario
        Task<IEnumerable<Usuario>> ObtenerUsuariosAsync();
        Task<Usuario?> ObtenerPorUsuarioIdAsync(int id);

        Task<Usuario?> AgregarUsuarioAsync(Usuario usuario); //Posiblemente se puede usar DTO
        Task<Usuario?> ActualizarUsuarioAsync(int id, Usuario usuario); //Se puede usar DTO

        Task DesactivarUsuarioAsync(int id);
    }
}
