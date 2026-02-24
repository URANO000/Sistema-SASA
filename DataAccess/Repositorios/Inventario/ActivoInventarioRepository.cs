using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Inventario
{
    public class ActivoInventarioRepository : IActivoInventarioRepository
    {
        private readonly ApplicationDbContext _db;
        public ActivoInventarioRepository(ApplicationDbContext db) => _db = db;

        public Task<ActivoInventario?> ObtenerPorIdAsync(int id) =>
            _db.ActivoInventario.FirstOrDefaultAsync(a => a.IdActivo == id);

        public Task<ActivoInventario?> ObtenerDetalleAsync(int id) =>
            _db.ActivoInventario
               .Include(a => a.TipoActivo)
               .Include(a => a.EstadoActivo)
               .FirstOrDefaultAsync(a => a.IdActivo == id);

        public async Task<List<ActivoInventario>> ListarAsync(string? q, int? estadoId, int? tipoId)
        {
            var query = _db.ActivoInventario
                .Include(a => a.TipoActivo)
                .Include(a => a.EstadoActivo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var t = q.Trim();
                query = query.Where(a =>
                    a.NumeroActivo.Contains(t) ||
                    (a.NombreMaquina != null && a.NombreMaquina.Contains(t)) ||
                    (a.SerieServicio != null && a.SerieServicio.Contains(t)));
            }

            if (estadoId.HasValue) query = query.Where(a => a.IdEstadoActivo == estadoId.Value);
            if (tipoId.HasValue) query = query.Where(a => a.IdTipoActivo == tipoId.Value);

            return await query.OrderByDescending(a => a.FechaCreacion).ToListAsync();
        }

        public Task<bool> ExisteNumeroActivoAsync(string numeroActivo) =>
            _db.ActivoInventario.AnyAsync(a => a.NumeroActivo == numeroActivo);

        public async Task<List<string>> ObtenerNumerosExistentesAsync(IEnumerable<string> numeros)
        {
            var list = numeros
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (list.Count == 0) return new List<string>();

            return await _db.ActivoInventario
                .Where(a => list.Contains(a.NumeroActivo))
                .Select(a => a.NumeroActivo)
                .ToListAsync();
        }

        public async Task CrearAsync(ActivoInventario entity) =>
            await _db.ActivoInventario.AddAsync(entity);

        public Task ActualizarAsync(ActivoInventario entity)
        {
            _db.ActivoInventario.Update(entity);
            return Task.CompletedTask;
        }

        public async Task AgregarRangoAsync(IEnumerable<ActivoInventario> entities) =>
            await _db.ActivoInventario.AddRangeAsync(entities);

        public Task GuardarAsync() => _db.SaveChangesAsync();
    }
}