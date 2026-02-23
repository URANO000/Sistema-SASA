using DataAccess.Modelos.Entidades.Integracion;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Integracion
{
    public class IntegracionHistorialRepository : IIntegracionHistorialRepository
    {
        private readonly ApplicationDbContext _context;
        public IntegracionHistorialRepository(ApplicationDbContext context) => _context = context;

        public Task<IntegracionHistorial?> ObtenerPorIdAsync(int id) =>
            _context.IntegracionHistorial.FirstOrDefaultAsync(h => h.Id == id);

        public async Task<int> CrearAsync(IntegracionHistorial hist)
        {
            _context.IntegracionHistorial.Add(hist);
            await _context.SaveChangesAsync();
            return hist.Id;
        }

        public Task ActualizarAsync(IntegracionHistorial hist)
        {
            _context.IntegracionHistorial.Update(hist);
            return Task.CompletedTask;
        }

        public Task<List<IntegracionHistorial>> ListarAsync() =>
            _context.IntegracionHistorial.AsNoTracking()
                .OrderByDescending(h => h.Fecha)
                .ToListAsync();

        public Task GuardarAsync() => _context.SaveChangesAsync();
    }
}