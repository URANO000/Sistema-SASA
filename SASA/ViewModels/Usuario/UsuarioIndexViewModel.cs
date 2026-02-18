namespace SASA.ViewModels.Usuario
{
    public class UsuarioIndexViewModel
    {
        //Primero el listado de usuarios
        public IReadOnlyList<UsuarioListaViewModel> Usuarios { get; init; } = [];
        public IReadOnlyList<string> RolesDisponibles { get; init; } = [];

        public CrearUsuarioViewModel CrearUsuario { get; set; } = new()
        {
            PrimerNombre = string.Empty,
            PrimerApellido = string.Empty,
            CorreoEmpresa = string.Empty,
            Departamento = string.Empty,
            Puesto = string.Empty,
            Rol = string.Empty
        };

        public UsuarioEditarViewModel EditarUsuario { get; set; } = new()
        {
            PrimerNombre = string.Empty,
            PrimerApellido = string.Empty,
            CorreoEmpresa = string.Empty,
            Departamento = string.Empty,
            Puesto = string.Empty,
            Rol = string.Empty
        };
    }
}
