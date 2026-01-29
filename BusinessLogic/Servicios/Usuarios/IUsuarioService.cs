using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.Entidades;

namespace BusinessLogic.Servicios.Usuarios
{
    public interface IUsuarioService
    {
        //Métodos para el servicio de Usuario
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync();
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(int id);

        Task<CrearUsuarioDto?> AgregarUsuarioAsync(CrearUsuarioDto usuario); //Posiblemente se puede usar DTO
        Task<Usuario?> ActualizarUsuarioAsync(int id, Usuario usuario); //Se puede usar DTO

        Task DesactivarUsuarioAsync(int id);
    }
}
