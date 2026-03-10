using DataAccess.Modelos.DTOs.Common;
using DataAccess.Modelos.DTOs.SubCategoria;

namespace DataAccess.Repositorios.SubCategorias
{
    public interface ISubCategoriaRepository
    {
        Task<PagedResultDto<ListaSubCategoriaDto>> ObtenerSubCategoriasAsync(FiltroSubCategoriaDto filtro);
        Task<bool> ExisteAsync(int idSubCategoria);
        Task<bool> ExisteNombreAsync(string nombreSubCategoria, int idCategoria, int? excluirIdSubCategoria = null);
        Task<int> CrearAsync(CrearSubCategoriaDto dto);
        Task<EditarSubCategoriaDto?> ObtenerParaEditarAsync(int idSubCategoria);
        Task<bool> EditarAsync(EditarSubCategoriaDto dto);
    }
}