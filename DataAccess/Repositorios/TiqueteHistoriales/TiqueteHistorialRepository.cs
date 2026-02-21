using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.Entidades.ModTiquete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositorios.TiqueteHistoriales
{
    public class TiqueteHistorialRepository : ITiqueteHistorialRepository
    {
        private readonly ApplicationDbContext _context;
        public TiqueteHistorialRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<TiqueteHistorial> AgregarTiqueteHistorialAsync(TiqueteHistorial tiquete)
        {
            _context.TiqueteHistoriales.Add(tiquete);
            await _context.SaveChangesAsync();
            return tiquete;
        }

        public async Task<List<TiqueteHistorialPorIdDto>> GetHistorialByTiqueteIdAsync(int idTiquete)
        {
            return await _context.TiqueteHistoriales
                .AsNoTracking()
                .Where(t => t.IdTiquete == idTiquete)
                .OrderByDescending(t => t.PerformedAt)
                .Select(t => new TiqueteHistorialPorIdDto
                {
                    IdHistorial = t.IdHistorial,
                    IdTiquete = t.IdTiquete,
                    TipoEvento = t.TipoEvento,
                    CampoAfectado = t.CampoAfectado,
                    ValorAnterior = t.ValorAnterior,
                    ValorNuevo = t.ValorNuevo,
                    DescripcionEvento = t.DescripcionEvento,
                    PerformedAt = t.PerformedAt,
                    PerformedBy = t.PerformedBy
                })
                .ToListAsync();
        }

    }
}
