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
        Task ActualizarUsuarioAsync(EditarUsuarioDto usuario);

        Task DesactivarUsuarioAsync(string id);

        Task ActivarUsuarioAsync(string id);

        Task<IEnumerable<UsuarioTIDropdownDto?>> ObtenerUsuariosTIAsync();
    }
}