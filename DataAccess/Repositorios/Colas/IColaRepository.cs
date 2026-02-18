namespace DataAccess.Repositorios.Colas
{
    public interface IColaRepository
    {
        Task<int> ObtenerColaPorCategoriaAsync(int idCategoria);
    }

}
