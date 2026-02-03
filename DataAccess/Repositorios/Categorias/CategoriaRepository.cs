

using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Categorias
{
    public class CategoriaRepository
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
    }
}
