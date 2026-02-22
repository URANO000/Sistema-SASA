namespace SASA.ViewModels.Shared
{
    public class MenuUsuarioViewModel
    {
        public bool EstaAutenticado { get; set; }
        public string NombreMostrar { get; set; } = "";
        public string Rol { get; set; } = "—";
    }
}
