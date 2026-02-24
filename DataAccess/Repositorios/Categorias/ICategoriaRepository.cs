

using DataAccess.Modelos.DTOs.Categoria;

namespace DataAccess.Repositorios.Categorias
{
    public interface ICategoriaRepository
    {
        Task<bool> ExisteAsync(int idCategoria);
        Task<IEnumerable<ListaCategoriaDto>> ObtenerCategoriaAsync();

        //Retorna el nombre -- Para historial
        Task<string?> GetNombreAsync(int idCategoria);
    }

}
