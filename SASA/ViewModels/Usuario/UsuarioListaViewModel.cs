namespace SASA.ViewModels.Usuario
{
    public class UsuarioListaViewModel
    {
        public string Id { get; init; } = default!;
        public string NombreCompleto { get; init; } = default!;
        public string? Departamento { get; init; }
        public string? Puesto { get; init; }
        public string? CorreoEmpresa { get; init; }
        public string Estado { get; init; } = default!;

        public DateTime? CreatedAt { get; init; } = default!;

        //Rol de usuario
        public string? Rol { get; set; }
    }
}
