using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Inventario
{
    public class ActivoInventarioTiqueteRepository : IActivoInventarioTiqueteRepository
    {
        private readonly ApplicationDbContext _db;

        public ActivoInventarioTiqueteRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<bool> ExisteAsociacionAsync(int idActivo, int idTiquete)
        {
            return _db.ActivoInventarioTiquete
                .AnyAsync(x => x.IdActivo == idActivo && x.IdTiquete == idTiquete);
        }

        public async Task CrearAsync(ActivoInventarioTiquete entity)
        {
            await _db.ActivoInventarioTiquete.AddAsync(entity);
        }

        public Task GuardarAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}