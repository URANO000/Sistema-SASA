

using DataAccess.Modelos.DTOs.Categoria;
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

        public async Task<bool> ExisteAsync(int idCategoria)
        {
            return await _context.Categorias
                .AsNoTracking()
                .AnyAsync(c => c.IdCategoria == idCategoria);
        }

        public async Task<IReadOnlyList<ListaCategoriaDto>> ObtenerCategoriaAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .Select(c => new ListaCategoriaDto
                {
                    IdCategoria = c.IdCategoria,
                    NombreCategoria = c.NombreCategoria
                })
                .ToListAsync();
        }
    }
}
