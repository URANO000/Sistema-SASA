using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Repositorios.Categorias;

namespace BusinessLogic.Servicios.Categorias
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;
        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        //Método para obtener todas las categorías
        public async Task<IEnumerable<ListaCategoriaDto>> ObtenerCategoriasAsync()
        {
            return await _repository.ObtenerCategoriaAsync();
        }
    }
}
