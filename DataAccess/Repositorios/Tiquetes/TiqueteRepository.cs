using DataAccess.Modelos.Entidades;
using DataAccess.Modelos.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Tiquetes
{
    public class TiqueteRepository : ITiqueteRepository
    {
        //Referencia al contexto de la base de datos
        private readonly ApplicationDbContext _context;

        public TiqueteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Implementación de los métodos del repositorio de tiquetes
        public async Task<IEnumerable<Tiquete>> ObtenerTiquetesAsync()
            => await _context.Tiquetes.ToListAsync();

        public async Task<Tiquete?> ObtenerTiquetePorIdAsync(int id)
            => await _context.Tiquetes.FindAsync(id);

        public async Task AgregarTiqueteAsync(Tiquete tiquete)
        {
            _context.Tiquetes.Add(tiquete);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarTiqueteAsync(Tiquete tiquete)
        {
            _context.Tiquetes.Update(tiquete);
            await _context.SaveChangesAsync();
        }

        public async Task CancelarTiquete(int id)
        {
            var tiquete = await _context.Tiquetes.FindAsync(id);
            if(tiquete != null)
            {
                tiquete.IdEstatus = (int)TiqueteEstatus.Cancelado;
                await _context.SaveChangesAsync();
            }
        }
    }
}
