using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.DTOs.TiqueteHistorial.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;
using DataAccess.Modelos.Enums;
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

        //Agregar tiquete historial para el servicio de tiquetes
        public async Task<TiqueteHistorial> AgregarTiqueteHistorialAsync(TiqueteHistorial tiquete)
        {
            _context.TiqueteHistoriales.Add(tiquete);
            await _context.SaveChangesAsync();
            return tiquete;
        }

        //Para ver en detalle de tiquetes
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
                    PerformedBy = t.User.CorreoEmpresa
                })
                .ToListAsync();
        }

        //Para ver en lista de acciones -- con paginación
        public async Task<PagedResult<ListaTiqueteHistorialDto>> ListarHistorialAsync(TiqueteHistorialFiltroDto filtro)
        {

            var query = _context.TiqueteHistoriales
                .AsNoTracking()
                .AsQueryable();

            //Filtrar si el searchbar no está vacío
            //Buscar por asunto de tiquete o correo de persona
            if (!string.IsNullOrEmpty(filtro.Search))
            {
                query = query.Where(t =>
                    t.Tiquete.Asunto.Contains(filtro.Search) ||
                    t.User.CorreoEmpresa.Contains(filtro.Search)
                    );
            }

            //Filtrar por tipo evento
            if(!string.IsNullOrWhiteSpace(filtro.TipoEvento))
            {
                if (Enum.TryParse<TipoEventoTiquete>(filtro.TipoEvento, true, out var tipoEventoParsed))
                {
                    query = query.Where(t => t.TipoEvento == tipoEventoParsed);
                }
                else
                {
                    query = query.Where(t => false);
                }
            }

            //Filtrar por fecha
            if (filtro.Fecha.HasValue)
            {
                var fecha = filtro.Fecha.Value.Date;

                query = query.Where(t => t.PerformedAt == fecha);
            }
            else if (filtro.FechaInicio.HasValue && filtro.FechaFinal.HasValue)
            {
                var inicio = filtro.FechaInicio.Value.Date;

                var fin = filtro.FechaFinal.Value.Date.AddDays(1);

                query = query.Where(t => t.PerformedAt >= inicio && t.PerformedAt < fin);

            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.PerformedAt)

                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(t => new ListaTiqueteHistorialDto
                {
                    IdHistorial = t.IdHistorial,
                    IdTiquete = t.IdTiquete,
                    TipoEvento = t.TipoEvento.ToString(),
                    CampoAfectado = t.CampoAfectado ?? "Sin campo afectado",
                    ValorAnterior = t.ValorAnterior ?? "Sin valor anterior",
                    ValorNuevo = t.ValorNuevo ?? "Sin valor nuevo",
                    DescripcionEvento = t.DescripcionEvento,
                    PerformedAt = t.PerformedAt,
                    PerformedBy = t.User.CorreoEmpresa
                })
                .ToListAsync();
            return new PagedResult<ListaTiqueteHistorialDto>
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filtro.PageSize)
            };
            
        }


    }
}
