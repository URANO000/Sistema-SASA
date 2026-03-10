using DataAccess.Modelos.DTOs.Common;
using DataAccess.Modelos.DTOs.SubCategoria;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Prioridad;
using DataAccess.Repositorios.SubCategorias;

namespace BusinessLogic.Servicios.SubCategorias
{
    public class SubCategoriaService : ISubCategoriaService
    {
        private readonly ISubCategoriaRepository _repository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IPrioridadRepository _prioridadRepository;
        public SubCategoriaService(ISubCategoriaRepository repository, ICategoriaRepository categoriaRepository, IPrioridadRepository prioridadRepository)
        {
            _repository = repository;
            _categoriaRepository = categoriaRepository;
            _prioridadRepository = prioridadRepository;
        }
        public async Task<IEnumerable<ListaSubCategoriasDto>> ObtenerSubCategoriasPorCategoria(int idCategoria)
        {
            return await _repository.ObtenerSubCategoriasPorCategoria(idCategoria);
        }

        public async Task<PagedResultDto<ListaSubCategoriaDto>> ObtenerSubCategoriasAsync(FiltroSubCategoriaDto filtro)
        {
            return await _repository.ObtenerSubCategoriasAsync(filtro);
        }


        public async Task<(bool Ok, string Mensaje)> CrearAsync(CrearSubCategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreSubCategoria))
                return (false, "El nombre de la subcategoría es requerido.");

            if (!await _categoriaRepository.ExisteAsync(dto.IdCategoria))
                return (false, "La categoría seleccionada no existe.");

            var prioridades = await _prioridadRepository.ObtenerPrioridadesAsync();
            if (!prioridades.Any(p => p.IdPrioridad == dto.IdPrioridad))
                return (false, "La prioridad seleccionada no existe.");

            var existe = await _repository.ExisteNombreAsync(dto.NombreSubCategoria, dto.IdCategoria);
            if (existe)
                return (false, "Ya existe una subcategoría con ese nombre en la categoría seleccionada.");

            await _repository.CrearAsync(dto);
            return (true, "Subcategoría creada correctamente.");
        }


        public async Task<EditarSubCategoriaDto?> ObtenerParaEditarAsync(int idSubCategoria)
        {
            return await _repository.ObtenerParaEditarAsync(idSubCategoria);
        }



        public async Task<(bool Ok, string Mensaje)> EditarAsync(EditarSubCategoriaDto dto)
        {
            if (dto.IdSubCategoria <= 0)
                return (false, "La subcategoría indicada no es válida.");

            if (string.IsNullOrWhiteSpace(dto.NombreSubCategoria))
                return (false, "El nombre de la subcategoría es requerido.");

            if (!await _categoriaRepository.ExisteAsync(dto.IdCategoria))
                return (false, "La categoría seleccionada no existe.");

            var prioridades = await _prioridadRepository.ObtenerPrioridadesAsync();
            if (!prioridades.Any(p => p.IdPrioridad == dto.IdPrioridad))
                return (false, "La prioridad seleccionada no existe.");

            var existe = await _repository.ExisteNombreAsync(
                dto.NombreSubCategoria,
                dto.IdCategoria,
                dto.IdSubCategoria);

            if (existe)
                return (false, "Ya existe otra subcategoría con ese nombre en la categoría seleccionada.");

            var actualizado = await _repository.EditarAsync(dto);
            if (!actualizado)
                return (false, "No se encontró la subcategoría a editar.");

            return (true, "Subcategoría actualizada correctamente.");
        }
    }
}
