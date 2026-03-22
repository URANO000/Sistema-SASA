using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;


namespace BusinessLogic.Servicios.Usuarios
{
    public interface IUsuarioService
    {
        //Métodos para el servicio de Usuario
        Task<PagedResult<ListaUsuarioDto>> ObtenerUsuariosAsync(UsuarioFiltroDto filtro);
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosReporteAsync();
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id);
        Task<PerfilUsuarioDto?> ObtenerPerfilAsync(string id);
        Task<ResultadoCreacionUsuarioDto> AgregarUsuarioAsync(CrearUsuarioDto usuario, string currentUserId);
        Task ActualizarUsuarioAsync(EditarUsuarioDto usuario, string currenUserId);

        Task DesactivarUsuarioAsync(string id, string currenUserId);

        Task ActivarUsuarioAsync(string id, string currentUserId);

        Task<IEnumerable<UsuarioTIDropdownDto?>> ObtenerUsuariosTIAsync();
    }
}