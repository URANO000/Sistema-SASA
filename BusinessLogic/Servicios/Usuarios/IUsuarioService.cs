using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;


namespace BusinessLogic.Servicios.Usuarios
{
    public interface IUsuarioService
    {
        //Métodos para el servicio de Usuario
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync();
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id);

        Task<ResultadoCreacionUsuarioDto> AgregarUsuarioAsync(CrearUsuarioDto usuario);
        Task ActualizarUsuarioAsync(EditarUsuarioDto usuario); //Se puede usar DTO

        Task DesactivarUsuarioAsync(string id);

        Task ActivarUsuarioAsync(string id);
    }
}