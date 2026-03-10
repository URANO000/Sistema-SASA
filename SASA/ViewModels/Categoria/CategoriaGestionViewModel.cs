using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Common;
using DataAccess.Modelos.DTOs.SubCategoria;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.Categoria
{
    public class CategoriaGestionViewModel
    {
        public string TabActiva { get; set; } = "categorias";

        public PagedResultDto<ListaCategoriaDto> CategoriasPaginadas { get; set; } = new();
        public PagedResultDto<ListaSubCategoriaDto> SubCategoriasPaginadas { get; set; } = new();

        public CrearCategoriaViewModel CrearCategoria { get; set; } = new();
        public CrearSubCategoriaViewModel CrearSubCategoria { get; set; } = new();

        public string? CategoriaBuscar { get; set; }
        public int CategoriaPagina { get; set; } = 1;

        public string? SubCategoriaBuscar { get; set; }
        public int? SubCategoriaFiltroIdCategoria { get; set; }
        public int? SubCategoriaFiltroIdPrioridad { get; set; }
        public int SubCategoriaPagina { get; set; } = 1;

        public List<SelectListItem> CategoriasDropdown { get; set; } = new();
        public List<SelectListItem> PrioridadesDropdown { get; set; } = new();
    }
}