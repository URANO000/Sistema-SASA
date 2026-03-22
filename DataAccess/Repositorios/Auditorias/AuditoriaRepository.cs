using DataAccess.Modelos.DTOs.Auditoria;
using DataAccess.Modelos.Entidades;

namespace DataAccess.Repositorios.Auditorias
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly ApplicationDbContext _context;
        public AuditoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //Para usuarios realmente
        public async Task AgregarAuditoria(Auditoria model)
        {
            _context.Auditorias.Add(model);
            await _context.SaveChangesAsync();
        }
    }
}
