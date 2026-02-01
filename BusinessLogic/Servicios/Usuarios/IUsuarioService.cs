using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.Entidades;
using DataAccess.Modelos.DTOs.Usuarios;


namespace BusinessLogic.Servicios.Usuarios
{
    public interface IUsuarioService
    {
        //Métodos para el servicio de Usuario
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync();
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id);

        Task<ResultadoCreacionUsuarioDto> AgregarUsuarioAsync(CrearUsuarioDto usuario);
        Task<ApplicationUser?> ActualizarUsuarioAsync(string id, ApplicationUser usuario); //Se puede usar DTO

        Task DesactivarUsuarioAsync(string id);
    }
}
