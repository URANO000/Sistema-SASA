using DataAccess.Modelos.DTOs.Common;
using DataAccess.Modelos.DTOs.SubCategoria;

namespace BusinessLogic.Servicios.SubCategorias
{
    public interface ISubCategoriaService
    {
        Task<IEnumerable<ListaSubCategoriaDto>> ObtenerSubCategoriasPorCategoria(int idCategoria);
        Task<PagedResultDto<ListaSubCategoriaDto>> ObtenerSubCategoriasAsync(FiltroSubCategoriaDto filtro);
        Task<(bool Ok, string Mensaje)> CrearAsync(CrearSubCategoriaDto dto);
        Task<EditarSubCategoriaDto?> ObtenerParaEditarAsync(int idSubCategoria);
        Task<(bool Ok, string Mensaje)> EditarAsync(EditarSubCategoriaDto dto);
    }
}
