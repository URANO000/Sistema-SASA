using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Inventario
{
    public class MantenimientoActivoRepository : IMantenimientoActivoRepository
    {
        private readonly ApplicationDbContext _db;

        public MantenimientoActivoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<MantenimientoActivo>> ListarAsync()
        {
            return await _db.MantenimientosActivos
                .Include(x => x.Activo)
                .OrderByDescending(x => x.FechaMantenimiento)
                .ToListAsync();
        }

        public async Task<List<MantenimientoActivo>> ListarPorActivoAsync(int idActivo)
        {
            return await _db.MantenimientosActivos
                .Include(x => x.Activo)
                .Where(x => x.IdActivo == idActivo)
                .OrderByDescending(x => x.FechaMantenimiento)
                .ToListAsync();
        }

        public async Task<MantenimientoActivo?> ObtenerPorIdAsync(int id)
        {
            return await _db.MantenimientosActivos
                .Include(x => x.Activo)
                .FirstOrDefaultAsync(x => x.IdMantenimiento == id);
        }

        public async Task<bool> ExisteMantenimientoEnProcesoAsync(int idActivo, int? excluirIdMantenimiento = null)
        {
            var query = _db.MantenimientosActivos
                .Where(x => x.IdActivo == idActivo && x.Estado == "En proceso");

            if (excluirIdMantenimiento.HasValue)
            {
                query = query.Where(x => x.IdMantenimiento != excluirIdMantenimiento.Value);
            }

            return await query.AnyAsync();
        }

        public async Task CrearAsync(MantenimientoActivo entity)
        {
            await _db.MantenimientosActivos.AddAsync(entity);
        }

        public Task ActualizarAsync(MantenimientoActivo entity)
        {
            _db.MantenimientosActivos.Update(entity);
            return Task.CompletedTask;
        }

        public Task GuardarAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
