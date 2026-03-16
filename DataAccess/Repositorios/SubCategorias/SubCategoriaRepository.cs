using DataAccess.Modelos.DTOs.Common;
using DataAccess.Modelos.DTOs.SubCategoria;
using DataAccess.Modelos.Entidades.ModTiquete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.SubCategorias
{
    public class SubCategoriaRepository : ISubCategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public SubCategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ListaSubCategoriaDto>> ObtenerSubCategoriasPorCategoria(int idCategoria)
        {
            return await _context.SubCategorias
                .AsNoTracking()
                .Where(s => s.IdCategoria == idCategoria)
                .Select(s => new ListaSubCategoriaDto
                {
                    IdSubCategoria = s.IdSubCategoria,
                    NombreSubCategoria = s.NombreSubCategoria,
                    IdPrioridad = s.IdPrioridad,
                    NombrePrioridad = s.Prioridad.NombrePrioridad
                })
                .ToListAsync();

            
        }

        public async Task<PagedResultDto<ListaSubCategoriaDto>> ObtenerSubCategoriasAsync(FiltroSubCategoriaDto filtro)
        {
            var query = _context.SubCategorias
                .AsNoTracking()
                .Include(s => s.Categoria)
                .Include(s => s.Prioridad)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro.Buscar))
            {
                var texto = filtro.Buscar.Trim();
                query = query.Where(s => s.NombreSubCategoria.Contains(texto));
            }

            if (filtro.IdCategoria.HasValue && filtro.IdCategoria.Value > 0)
                query = query.Where(s => s.IdCategoria == filtro.IdCategoria.Value);

            if (filtro.IdPrioridad.HasValue && filtro.IdPrioridad.Value > 0)
                query = query.Where(s => s.IdPrioridad == filtro.IdPrioridad.Value);

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Categoria!.NombreCategoria)
                .ThenBy(s => s.NombreSubCategoria)
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(s => new ListaSubCategoriaDto
                {
                    IdSubCategoria = s.IdSubCategoria,
                    NombreSubCategoria = s.NombreSubCategoria,
                    IdCategoria = s.IdCategoria,
                    NombreCategoria = s.Categoria != null ? s.Categoria.NombreCategoria : null,
                    IdPrioridad = s.IdPrioridad,
                    NombrePrioridad = s.Prioridad != null ? s.Prioridad.NombrePrioridad : null,
                    DuracionMinutos = s.Prioridad != null ? s.Prioridad.DuracionMinutos : null
                })
                .ToListAsync();

            return new PagedResultDto<ListaSubCategoriaDto>
            {
                Items = items,
                TotalRecords = totalRecords,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize
            };
        }

        public async Task<bool> ExisteAsync(int idSubCategoria)
        {
            return await _context.SubCategorias
                .AsNoTracking()
                .AnyAsync(s => s.IdSubCategoria == idSubCategoria);
        }

        public async Task<bool> ExisteNombreAsync(string nombreSubCategoria, int idCategoria, int? excluirIdSubCategoria = null)
        {
            nombreSubCategoria = nombreSubCategoria.Trim();

            return await _context.SubCategorias
                .AsNoTracking()
                .AnyAsync(s =>
                    s.NombreSubCategoria == nombreSubCategoria &&
                    s.IdCategoria == idCategoria &&
                    (!excluirIdSubCategoria.HasValue || s.IdSubCategoria != excluirIdSubCategoria.Value));
        }

        public async Task<int> CrearAsync(CrearSubCategoriaDto dto)
        {
            var entity = new SubCategoria
            {
                IdCategoria = dto.IdCategoria,
                IdPrioridad = dto.IdPrioridad,
                NombreSubCategoria = dto.NombreSubCategoria.Trim()
            };

            _context.SubCategorias.Add(entity);
            await _context.SaveChangesAsync();
            return entity.IdSubCategoria;
        }

        public async Task<EditarSubCategoriaDto?> ObtenerParaEditarAsync(int idSubCategoria)
        {
            return await _context.SubCategorias
                .AsNoTracking()
                .Where(s => s.IdSubCategoria == idSubCategoria)
                .Select(s => new EditarSubCategoriaDto
                {
                    IdSubCategoria = s.IdSubCategoria,
                    IdCategoria = s.IdCategoria ?? 0,
                    IdPrioridad = s.IdPrioridad ?? 0,
                    NombreSubCategoria = s.NombreSubCategoria
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> EditarAsync(EditarSubCategoriaDto dto)
        {
            var entity = await _context.SubCategorias
                .FirstOrDefaultAsync(s => s.IdSubCategoria == dto.IdSubCategoria);

            if (entity == null)
                return false;

            entity.IdCategoria = dto.IdCategoria;
            entity.IdPrioridad = dto.IdPrioridad;
            entity.NombreSubCategoria = dto.NombreSubCategoria.Trim();

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
