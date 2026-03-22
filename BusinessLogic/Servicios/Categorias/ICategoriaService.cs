using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Common;

namespace BusinessLogic.Servicios.Categorias
{
    public interface ICategoriaService
    {
        Task<PagedResultDto<ListaCategoriaDto>> ObtenerCategoriasAsync(FiltroCategoriaDto filtro);
        Task<List<ListaCategoriaDto>> ObtenerTodasAsync();
        Task<(bool Ok, string Mensaje)> CrearAsync(CrearCategoriaDto dto);
        Task<EditarCategoriaDto?> ObtenerParaEditarAsync(int idCategoria);
        Task<(bool Ok, string Mensaje)> EditarAsync(EditarCategoriaDto dto);
    }
}