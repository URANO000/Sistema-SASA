using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;


namespace DataAccess.Repositorios.Usuarios
{
    public interface IUsuarioRepository
    {
        Task<PagedResult<ListaUsuarioDto>> ObtenerUsuariosAsync(UsuarioFiltroDto filtro); //Listar
        Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosReporteAsync(); //Lista de todos los usuarios in pg
        Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id); //Detalle
        Task ActualizarUsuarioAsync(ApplicationUser usuario); //Actualizar
        Task DesactivarUsuario(string id); //Desactivar. Nunca eliminar
        Task ActivarUsuario(string id); //Activar usuario desactivado

        Task<IReadOnlyList<UsuarioTIDropdownDto?>> ObtenerUsuariosTIAsync(); //Obtener usuarios de TI

    }
}
