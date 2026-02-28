namespace SASA.ViewModels.Usuario
{
    public class UsuarioDetalleViewModel
    {
        public string Id { get; init; } = default!;
        public string NombreCompleto { get; init; } = default!;
        public string? Departamento { get; init; }
        public string? Puesto { get; init; }
        public string? CorreoEmpresa { get; init; }
        public string Estado { get; init; } = default!;

        //Rol de usuario
        public string? Rol { get; init; } = default!;

        //Intentos de login
        public List<LoginAttemptItemViewModel> IntentosLogin { get; set; } = new();
    }
}
