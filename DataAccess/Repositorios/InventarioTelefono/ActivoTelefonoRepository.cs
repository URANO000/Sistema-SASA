using DataAccess;
using DataAccess.Modelos.Entidades.InventarioTelefono;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.InventarioTelefono
{
    public class ActivoTelefonoRepository : IActivoTelefonoRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivoTelefonoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActivoTelefono?> ObtenerPorIdAsync(int id)
        {
            return await _context.ActivoTelefono
                .FirstOrDefaultAsync(x => x.IdActivoTelefono == id);
        }

        public async Task<int> ContarAsync(string? texto)
        {
            var query = _context.ActivoTelefono.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(x =>
                    x.NombreColaborador.Contains(texto) ||
                    (x.NumeroCelular != null && x.NumeroCelular.Contains(texto)));
            }

            return await query.CountAsync();
        }

        public async Task<List<ActivoTelefono>> ListarPaginadoAsync(
            string? texto,
            int skip,
            int take,
            string sortBy,
            string sortDir)
        {
            var query = _context.ActivoTelefono.AsQueryable();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                query = query.Where(x =>
                    x.NombreColaborador.Contains(texto) ||
                    (x.NumeroCelular != null && x.NumeroCelular.Contains(texto)));
            }

            query = sortDir == "desc"
                ? query.OrderByDescending(x => x.NombreColaborador)
                : query.OrderBy(x => x.NombreColaborador);

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> ExisteImeiAsync(string imei)
        {
            if (string.IsNullOrWhiteSpace(imei))
                return false;

            return await _context.ActivoTelefono
                .AnyAsync(x => x.IMEI == imei);
        }

        public async Task<bool> ExisteImeiAsync(string imei, int idExcluir)
        {
            if (string.IsNullOrWhiteSpace(imei))
                return false;

            return await _context.ActivoTelefono
                .AnyAsync(x => x.IMEI == imei && x.IdActivoTelefono != idExcluir);
        }

        public async Task CrearAsync(ActivoTelefono entity)
        {
            await _context.ActivoTelefono.AddAsync(entity);
        }

        public Task ActualizarAsync(ActivoTelefono entity)
        {
            if (entity.IdActivoTelefono <= 0)
                throw new InvalidOperationException("El Id del teléfono no es válido para actualizar.");

            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}