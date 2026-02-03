using DataAccess.Modelos.Entidades;
using DataAccess.Modelos.DTOs.Tiquete;
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
        public async Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesAsync()
        {
            return await _context.Tiquetes
                .AsNoTracking()
                .Select(t => new ListaTiqueteDTO
                {
                    IdTiquete = t.IdTiquete,
                    Asunto = t.Asunto,
                    Descripcion = t.Descripcion,
                    Resolucion = t.Resolucion ?? "Sin Resolución",

                    Estatus = t.Estatus.NombreEstatus,
                    Prioridad = t.Prioridad != null ? t.Prioridad.NombrePrioridad : "Sin prioridad",
                    Categoria = t.Categoria.NombreCategoria,
                    Cola = t.Cola != null ? t.Cola.NombreCola : "Sin Cola",

                    ReportedBy = t.ReportedBy.CorreoEmpresa,
                    Asignee = t.Asignee != null ? t.Asignee.CorreoEmpresa : "Sin asignar",

                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt

                })
                .ToListAsync();
        }

        public async Task<ListaTiqueteDTO?> ObtenerTiquetePorIdAsync(int id)
        {
            return await _context.Tiquetes
                .AsNoTracking()
                .Where(t => t.IdTiquete == id)
                .Select(t => new ListaTiqueteDTO
                {
                    IdTiquete = t.IdTiquete,
                    Asunto = t.Asunto,
                    Descripcion = t.Descripcion,
                    Resolucion = t.Resolucion,

                    Estatus = t.Estatus.NombreEstatus,
                    Prioridad = t.Prioridad != null ? t.Prioridad.NombrePrioridad : "Sin prioridad",
                    Categoria = t.Categoria.NombreCategoria,
                    Cola = t.Cola != null ? t.Cola.NombreCola : "Sin Cola",
                    ReportedBy = t.ReportedBy.CorreoEmpresa,
                    Asignee = t.Asignee != null ? t.Asignee.CorreoEmpresa : "Sin asignar",
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

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
