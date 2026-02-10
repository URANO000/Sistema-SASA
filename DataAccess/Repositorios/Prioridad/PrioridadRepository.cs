using DataAccess.Modelos.DTOs.Prioridad;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositorios.Prioridad
{
    public class PrioridadRepository : IPrioridadRepository
    {
        //Referencia a app db context
        private readonly ApplicationDbContext _context;
        public PrioridadRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<ListaPrioridadDto>> ObtenerPrioridadesAsync()
        {
            return await _context.Prioridades
                .AsNoTracking()
                .Select(p => new ListaPrioridadDto
                {
                    IdPrioridad = p.IdPrioridad,
                    NombrePrioridad = p.NombrePrioridad
                })
                .ToListAsync();
        }
    }
}
