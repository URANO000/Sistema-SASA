

namespace DataAccess.Repositorios.Categorias
{
    public interface ICategoriaRepository
    {
        Task<bool> ExisteAsync(int idCategoria);
    }

}
