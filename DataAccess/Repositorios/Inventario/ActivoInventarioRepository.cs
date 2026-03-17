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
                .Include(a => a.EstadoActivo)
                .Include(a => a.TipoActivo)
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

        public async Task<int> ContarAsync(string? q, int? estadoId, int? tipoId)
        {
            var query = _db.ActivoInventario.AsQueryable();

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

            return await query.CountAsync();
        }

        // ✅ Ordenamiento + paginación reales (OrderBy antes de Skip/Take)
        public async Task<List<ActivoInventario>> ListarPaginadoAsync(
            string? q,
            int? estadoId,
            int? tipoId,
            int skip,
            int take,
            string sortBy,
            string sortDir)
        {
            var query = _db.ActivoInventario
                .Include(a => a.EstadoActivo)
                .Include(a => a.TipoActivo)
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

            bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

            query = (sortBy ?? "Codigo") switch
            {
                "Nombre" => desc ? query.OrderByDescending(a => a.NombreMaquina)
                                 : query.OrderBy(a => a.NombreMaquina),

                "Tipo" => desc ? query.OrderByDescending(a => a.TipoActivo!.Nombre)
                                 : query.OrderBy(a => a.TipoActivo!.Nombre),

                "Estado" => desc ? query.OrderByDescending(a => a.EstadoActivo!.Nombre)
                                 : query.OrderBy(a => a.EstadoActivo!.Nombre),

                _ => desc ? query.OrderByDescending(a => a.NumeroActivo)   // "Codigo"
                                 : query.OrderBy(a => a.NumeroActivo),
            };

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public Task<bool> ExisteNumeroActivoAsync(string numeroActivo) =>
            _db.ActivoInventario.AnyAsync(a => a.NumeroActivo == numeroActivo);

        public async Task<List<string>> ObtenerNumerosExistentesAsync(IEnumerable<string> numeros)
        {
            var set = numeros.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (set.Count == 0) return new List<string>();

            return await _db.ActivoInventario
                .Where(a => set.Contains(a.NumeroActivo))
                .Select(a => a.NumeroActivo)
                .ToListAsync();
        }

        public async Task CrearAsync(ActivoInventario entity) => await _db.ActivoInventario.AddAsync(entity);

        public Task ActualizarAsync(ActivoInventario entity)
        {
            _db.ActivoInventario.Update(entity);
            return Task.CompletedTask;
        }

        public async Task AgregarRangoAsync(IEnumerable<ActivoInventario> entities) =>
            await _db.ActivoInventario.AddRangeAsync(entities);

        public Task GuardarAsync() => _db.SaveChangesAsync();

        //--------------------------------Para Dashboard-------------------------------------------
        public async Task<int> ContarInventarioAsync()
        {
            return await _db.ActivoInventario.CountAsync();
        }
    }
}