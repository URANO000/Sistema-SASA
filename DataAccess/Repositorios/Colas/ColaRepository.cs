using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Colas
{
    public class ColaRepository
    {
        private readonly ApplicationDbContext _context;
        public ColaRepository(ApplicationDbContext context)
        {
            _context =  context;
        }

        public async Task<int> ObtenerColaPorCategoriaAsync(int idCategoria)
        {
            var colaId = await _context.Colas
                .AsNoTracking()
                .Where(c =>
                    c.IdCategoria == idCategoria &&
                    c.IsActive)
                .OrderBy(c => c.IdCola) //Asegura que se obtenga la cola con el Id más bajo
                .Select(c => c.IdCola)
                .FirstOrDefaultAsync();

            if (colaId == 0)
                throw new InvalidOperationException(
                    "No existe una cola activa para la categoría seleccionada");

            return colaId;
        }
    }
}
