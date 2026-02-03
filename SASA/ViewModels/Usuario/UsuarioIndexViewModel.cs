namespace SASA.ViewModels.Usuario
{
    public class UsuarioIndexViewModel
    {
        public IReadOnlyList<UsuarioListaViewModel> Usuarios { get; init; } = [];
        public IReadOnlyList<string> RolesDisponibles { get; init; } = [];
    }
}
