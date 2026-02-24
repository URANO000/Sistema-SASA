using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Inventario
{
    public class ActivoInventarioRepository : IActivoInventarioRepository
    {
        private readonly ApplicationDbContext _context;
        public ActivoInventarioRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<string>> ObtenerNumerosExistentesAsync(IEnumerable<string> numeros)
        {
            var lista = numeros.Distinct().ToList();
            if (lista.Count == 0) return new List<string>();

            return await _context.ActivoInventario.AsNoTracking()
                .Where(a => lista.Contains(a.NumeroActivo))
                .Select(a => a.NumeroActivo)
                .ToListAsync();
        }

        public Task<bool> ExisteNumeroAsync(string numeroActivo) =>
            _context.ActivoInventario.AnyAsync(a => a.NumeroActivo == numeroActivo);

        public Task AgregarRangoAsync(IEnumerable<ActivoInventario> activos)
        {
            _context.ActivoInventario.AddRange(activos);
            return Task.CompletedTask;
        }

        public Task GuardarAsync() => _context.SaveChangesAsync();
    }
}