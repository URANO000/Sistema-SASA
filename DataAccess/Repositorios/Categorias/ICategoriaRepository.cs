using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Common;

namespace DataAccess.Repositorios.Categorias
{
    public interface ICategoriaRepository
    {
        Task<PagedResultDto<ListaCategoriaDto>> ObtenerCategoriaAsync(FiltroCategoriaDto filtro);
        Task<List<ListaCategoriaDto>> ObtenerTodasAsync();
        Task<bool> ExisteAsync(int idCategoria);

        //Retorna el nombre -- Para historial
        Task<string?> GetNombreAsync(int idCategoria);
        Task<bool> ExisteNombreAsync(string nombreCategoria, int? excluirIdCategoria = null);
        Task<int> CrearAsync(CrearCategoriaDto dto);
        Task<EditarCategoriaDto?> ObtenerParaEditarAsync(int idCategoria);
        Task<bool> EditarAsync(EditarCategoriaDto dto);
    }
}