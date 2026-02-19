namespace SASA.ViewModels.Usuario.Extras
{
    public class UsuarioFiltroViewModel
    {
        public string? Search { get; set; }
        public string? Departamento { get; set; }
        public bool? Estado { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public bool TieneAnterior => PageNumber > 1;
        public bool TieneSiguiente => PageNumber < TotalPages;

    }
}
