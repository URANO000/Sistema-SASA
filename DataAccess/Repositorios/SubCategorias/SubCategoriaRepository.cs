using DataAccess.Modelos.DTOs.SubCategoria;
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
        public async Task<IEnumerable<ListaSubCategoriasDto>> ObtenerSubCategoriasPorCategoria(int idCategoria)
        {
            return await _context.SubCategorias
                .AsNoTracking()
                .Where(s => s.IdCategoria == idCategoria)
                .Select(s => new ListaSubCategoriasDto
                {
                    IdSubCategoria = s.IdSubCategoria,
                    NombreSubCategoria = s.NombreSubCategoria
                })
                .ToListAsync();

            
        }
    }
}
