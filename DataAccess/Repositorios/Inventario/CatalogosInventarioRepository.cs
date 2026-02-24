using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Inventario
{
    public class CatalogosInventarioRepository : ICatalogosInventarioRepository
    {
        private readonly ApplicationDbContext _context;
        public CatalogosInventarioRepository(ApplicationDbContext context) => _context = context;

        public async Task<HashSet<string>> ObtenerTiposSetAsync()
        {
            var tipos = await _context.TipoActivoInventario.AsNoTracking()
                .Select(t => t.Nombre).ToListAsync();
            return new HashSet<string>(tipos, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<HashSet<string>> ObtenerLicenciasSetAsync()
        {
            var lic = await _context.TipoLicenciaInventario.AsNoTracking()
                .Select(l => l.Nombre).ToListAsync();
            return new HashSet<string>(lic, StringComparer.OrdinalIgnoreCase);
        }

        public Task<List<TipoActivoInventario>> ObtenerTiposAsync() =>
            _context.TipoActivoInventario.AsNoTracking().ToListAsync();

        public Task<List<EstadoActivoInventario>> ObtenerEstadosAsync() =>
            _context.EstadoActivoInventario.AsNoTracking().ToListAsync();

        public Task<List<TipoLicenciaInventario>> ObtenerLicenciasAsync() =>
            _context.TipoLicenciaInventario.AsNoTracking().ToListAsync();
    }
}