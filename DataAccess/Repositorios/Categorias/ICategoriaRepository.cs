

using DataAccess.Modelos.DTOs.Categoria;

namespace DataAccess.Repositorios.Categorias
{
    public interface ICategoriaRepository
    {
        public Task<bool> ExisteAsync(int idCategoria);
        public Task<IReadOnlyList<ListaCategoriaDto>> ObtenerCategoriaAsync();
    }

}
