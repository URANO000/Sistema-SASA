using DataAccess.Modelos.DTOs.SubCategoria;

namespace DataAccess.Repositorios.SubCategorias
{
    public interface ISubCategoriaRepository
    {
        Task<IEnumerable<ListaSubCategoriasDto>> ObtenerSubCategoriasPorCategoria(int idCategoria);
    }
}
