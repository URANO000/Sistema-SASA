using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Common;
using DataAccess.Modelos.Entidades.ModTiquete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Categorias
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<ListaCategoriaDto>> ObtenerCategoriaAsync(FiltroCategoriaDto filtro)
        {
            var query = _context.Categorias.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Buscar))
            {
                var texto = filtro.Buscar.Trim();
                query = query.Where(c => c.NombreCategoria.Contains(texto));
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.NombreCategoria)
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(c => new ListaCategoriaDto
                {
                    IdCategoria = c.IdCategoria,
                    NombreCategoria = c.NombreCategoria
                })
                .ToListAsync();

            return new PagedResultDto<ListaCategoriaDto>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize
            };
        }

        public async Task<List<ListaCategoriaDto>> ObtenerTodasAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.NombreCategoria)
                .Select(c => new ListaCategoriaDto
                {
                    IdCategoria = c.IdCategoria,
                    NombreCategoria = c.NombreCategoria
                })
                .ToListAsync();
        }

        public async Task<bool> ExisteAsync(int idCategoria)
        {
            return await _context.Categorias
                .AsNoTracking()
                .AnyAsync(c => c.IdCategoria == idCategoria);
        }

        public async Task<string?> GetNombreAsync(int idCategoria)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.IdCategoria == idCategoria)
                .Select(c => c.NombreCategoria)
                .FirstOrDefaultAsync(); //No retorna nulos
        }

        public async Task<bool> ExisteNombreAsync(string nombreCategoria, int? excluirIdCategoria = null)
        {
            nombreCategoria = nombreCategoria.Trim();

            return await _context.Categorias
                .AsNoTracking()
                .AnyAsync(c =>
                    c.NombreCategoria == nombreCategoria &&
                    (!excluirIdCategoria.HasValue || c.IdCategoria != excluirIdCategoria.Value));
        }

        public async Task<int> CrearAsync(CrearCategoriaDto dto)
        {
            var entity = new Categoria
            {
                NombreCategoria = dto.NombreCategoria.Trim()
            };

            _context.Categorias.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdCategoria;
        }

        public async Task<EditarCategoriaDto?> ObtenerParaEditarAsync(int idCategoria)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.IdCategoria == idCategoria)
                .Select(c => new EditarCategoriaDto
                {
                    IdCategoria = c.IdCategoria,
                    NombreCategoria = c.NombreCategoria
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EditarAsync(EditarCategoriaDto dto)
        {
            var entity = await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == dto.IdCategoria);

            if (entity == null)
                return false;

            entity.NombreCategoria = dto.NombreCategoria.Trim();
            await _context.SaveChangesAsync();
            return true;
        }
    }
}