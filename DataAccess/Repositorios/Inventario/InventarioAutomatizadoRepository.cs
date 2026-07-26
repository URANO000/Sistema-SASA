using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Inventario
{
    public class InventarioAutomatizadoRepository
        : IInventarioAutomatizadoRepository
    {
        private readonly ApplicationDbContext _db;

        public InventarioAutomatizadoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<InventarioAutomatizado?> ObtenerExistenteAsync(
            string? serialPC,
            string nombreEquipo)
        {
            var serialNormalizado = serialPC?.Trim().ToUpper();
            var nombreNormalizado = nombreEquipo.Trim().ToUpper();

            if (!string.IsNullOrWhiteSpace(serialNormalizado))
            {
                return await _db.InventariosAutomatizados
                    .FirstOrDefaultAsync(x =>
                        x.SerialPC != null &&
                        x.SerialPC.ToUpper() == serialNormalizado);
            }

            return await _db.InventariosAutomatizados
                .FirstOrDefaultAsync(x =>
                    x.NombreEquipo != null &&
                    x.NombreEquipo.ToUpper() == nombreNormalizado);
        }

        public async Task AgregarAsync(
            InventarioAutomatizado inventarioAutomatizado)
        {
            await _db.InventariosAutomatizados
                .AddAsync(inventarioAutomatizado);
        }

        public Task GuardarAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}