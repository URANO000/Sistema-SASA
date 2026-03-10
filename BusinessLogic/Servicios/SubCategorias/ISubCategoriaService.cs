

using DataAccess.Modelos.DTOs.SubCategoria;

namespace BusinessLogic.Servicios.SubCategorias
{
    public interface ISubCategoriaService
    {
        Task<IEnumerable<ListaSubCategoriasDto>> ObtenerSubCategoriasPorCategoria(int idCategoria);
    }
}
