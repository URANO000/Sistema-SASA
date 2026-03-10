using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Common;
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

        public async Task<PagedResultDto<ListaCategoriaDto>> ObtenerCategoriasAsync(FiltroCategoriaDto filtro)
        {
            return await _repository.ObtenerCategoriaAsync(filtro);
        }

        //Método para obtener todas las categorías
        public async Task<List<ListaCategoriaDto>> ObtenerTodasAsync()
        {
            return await _repository.ObtenerTodasAsync();
        }

        public async Task<(bool Ok, string Mensaje)> CrearAsync(CrearCategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreCategoria))
                return (false, "El nombre de la categoría es requerido.");

            var existe = await _repository.ExisteNombreAsync(dto.NombreCategoria);
            if (existe)
                return (false, "Ya existe una categoría con ese nombre.");

            await _repository.CrearAsync(dto);
            return (true, "Categoría creada correctamente.");
        }

        public async Task<EditarCategoriaDto?> ObtenerParaEditarAsync(int idCategoria)
        {
            return await _repository.ObtenerParaEditarAsync(idCategoria);
        }

        public async Task<(bool Ok, string Mensaje)> EditarAsync(EditarCategoriaDto dto)
        {
            if (dto.IdCategoria <= 0)
                return (false, "La categoría indicada no es válida.");

            if (string.IsNullOrWhiteSpace(dto.NombreCategoria))
                return (false, "El nombre de la categoría es requerido.");

            var existe = await _repository.ExisteNombreAsync(dto.NombreCategoria, dto.IdCategoria);
            if (existe)
                return (false, "Ya existe otra categoría con ese nombre.");

            var actualizado = await _repository.EditarAsync(dto);
            if (!actualizado)
                return (false, "No se encontró la categoría a editar.");

            return (true, "Categoría actualizada correctamente.");
        }
    }
}