using DataAccess.Modelos.DTOs.SubCategoria;
using DataAccess.Repositorios.SubCategorias;

namespace BusinessLogic.Servicios.SubCategorias
{
    public class SubCategoriaService : ISubCategoriaService
    {
        private readonly ISubCategoriaRepository _repository;
        public SubCategoriaService(ISubCategoriaRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<ListaSubCategoriasDto>> ObtenerSubCategoriasPorCategoria(int idCategoria)
        {
            return await _repository.ObtenerSubCategoriasPorCategoria(idCategoria);
        }
    }
}
