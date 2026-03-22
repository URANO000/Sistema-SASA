using DataAccess.Modelos.DTOs.Avances;
using DataAccess.Modelos.Entidades.ModTiquete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Avances
{
    public class AvanceRepository : IAvanceRepository
    {
        //Referencia all app db context para interactuar con la tabla Avance
        private readonly ApplicationDbContext _context;
        public AvanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Avance> AgregarAvanceAsync(Avance avance)
        {
            _context.Avances.Add(avance);
            await _context.SaveChangesAsync();
            return avance;
        }

        public async Task<List<ListaAvancesDto>> ListaAvancesPorTiqueteAsync(int idTiquete)
        {
            return await _context.Avances
                .AsNoTracking()
                .Where(a => a.IdTiquete == idTiquete)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ListaAvancesDto
                {
                    Autor = a.Autor.CorreoEmpresa,
                    TextoAvance = a.TextoAvance,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();
        }
    }
}
