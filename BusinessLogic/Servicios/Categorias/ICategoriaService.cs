using DataAccess.Modelos.DTOs.Categoria;

namespace BusinessLogic.Servicios.Categorias
{
    public interface ICategoriaService
    {
        public Task<IReadOnlyList<ListaCategoriaDto>> ObtenerCategoriasAsync();
    }
}
