using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;
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
        public async Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro, string? currentUserId = null)
        {
            var query = _context.Tiquetes
                .AsNoTracking()
                .AsQueryable();

            /*Si el id no es nulp, es que en el controlador se define que es un empleado normal
            Por lo tanto, se hace un where por id*/
            if (!string.IsNullOrEmpty(currentUserId))
            {
                query = query.Where(t => t.IdReportedBy == currentUserId);
            }

            //Filtrar si el searchbar no está vacío
            if (!string.IsNullOrWhiteSpace(filtro.Search))
            {
                query = query.Where(t =>
                    t.Asunto.Contains(filtro.Search) ||
                    t.Descripcion.Contains(filtro.Search));
            }

            //Si el filtro de estatus no es vacío
            if (!string.IsNullOrWhiteSpace(filtro.Estatus))
            {
                query = query.Where(t =>
                    t.Estatus.NombreEstatus.Replace(" ", "") == filtro.Estatus);
            }


            //Si el filtro de Fecha no es vacío
            if (filtro.Fecha.HasValue)
            {
                var fecha = filtro.Fecha.Value.Date;

                query = query.Where(t => t.CreatedAt == fecha);
            }
            else if (filtro.FechaInicio.HasValue && filtro.FechaFinal.HasValue)
            {
                var inicio = filtro.FechaInicio.Value.Date; //Sólo el date, no la hora
                //Ésta lógica es para atrapar todo ese día de la fecha final
                var fin = filtro.FechaFinal.Value.Date.AddDays(1);

                query = query.Where(t => t.CreatedAt >= inicio && t.CreatedAt < fin);

            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.CreatedAt) //Primero los tiquetes recien creados


                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .Select(t => new ListaTiqueteDTO
                {
                    IdTiquete = t.IdTiquete,
                    Asunto = t.Asunto,
                    Descripcion = t.Descripcion,
                    Resolucion = t.Resolucion ?? "Sin Resolución",
                    Estatus = t.Estatus != null ? t.Estatus.NombreEstatus : "Sin estatus",
                    Categoria = t.Categoria != null ? t.Categoria.NombreCategoria : "Sin categoría",
                    ReportedBy = t.ReportedBy.CorreoEmpresa,
                    Departamento = t.ReportedBy.Departamento,
                    Asignee = t.Asignee != null ? t.Asignee.CorreoEmpresa : "Sin asignar",
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return new PagedResult<ListaTiqueteDTO>
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filtro.PageSize)
            };
        }


        //Para reportes ----- Lista de todos los tiquetes -----------------------------
        public async Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesReporteAsync()
        {
            return await _context.Tiquetes
                .AsNoTracking()
                .Select(t => new ListaTiqueteDTO
                {
                    IdTiquete = t.IdTiquete,
                    Asunto = t.Asunto,
                    Descripcion = t.Descripcion,
                    Resolucion = t.Resolucion ?? "Sin Resolución",

                    Estatus = t.Estatus != null ? t.Estatus.NombreEstatus : "Sin estatus",
                    Categoria = t.Categoria != null ? t.Categoria.NombreCategoria : "Sin categoría",

                    ReportedBy = t.ReportedBy != null ? t.ReportedBy.CorreoEmpresa : "Desconocido",
                    Asignee = t.Asignee != null ? t.Asignee.CorreoEmpresa : "Sin asignar",

                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();
        }

        //Para detalles----------------------------------------------------------

        public async Task<DetalleTiqueteDto?> ObtenerTiquetePorIdReadAsync(int id)
        {
            return await _context.Tiquetes
                .AsNoTracking()
                .Where(t => t.IdTiquete == id)
                .Select(t => new DetalleTiqueteDto
                {
                    IdTiquete = t.IdTiquete,
                    Asunto = t.Asunto,
                    Descripcion = t.Descripcion,
                    Resolucion = t.Resolucion != null ? t.Resolucion : "Sin Resolución",

                    Estatus = t.Estatus.NombreEstatus,
                    Categoria = t.Categoria.NombreCategoria,
                    ReportedBy = t.ReportedBy.CorreoEmpresa,
                    Departamento = t.ReportedBy.Departamento,
                    Asignee = t.Asignee != null ? t.Asignee.CorreoEmpresa : "Sin asignar",
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        //Obtener toda la entidad por ID, para editar --------------
        public async Task<Tiquete?> ObtenerEntidadPorIdAsync(int id)
        {
            return await _context.Tiquetes
                .FirstOrDefaultAsync(t => t.IdTiquete == id);
        }

        //Obtener tiquete por ID, editar también ----------------------------
        public async Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id)
        {
            return await _context.Tiquetes
        .AsNoTracking()
        .Where(t => t.IdTiquete == id)
        .Select(t => new TiquetePorIdDto
        {
            IdTiquete = t.IdTiquete,
            Asunto = t.Asunto,
            Descripcion = t.Descripcion,
            IdCategoria = t.IdCategoria,
            IdEstatus = t.IdEstatus,
            IdAsignee = t.IdAsignee,
            Resolucion = t.Resolucion,
            ReportedByEmail = t.ReportedBy.CorreoEmpresa,
            ReportedByNombre = (t.ReportedBy.PrimerNombre ?? "") + " " + (t.ReportedBy.PrimerApellido ?? ""),
            EstatusNombre = t.Estatus.NombreEstatus
        })
        .FirstOrDefaultAsync();
        }

        public async Task<Tiquete> AgregarTiqueteAsync(Tiquete tiquete)
        {
            _context.Tiquetes.Add(tiquete);
            await _context.SaveChangesAsync();
            return tiquete;
        }

        public async Task ActualizarTiqueteAsync(Tiquete tiquete)
        {
            //_context.Tiquetes.Update(tiquete);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteTiquete(int id)
        {
            return await _context.Tiquetes
                .AsNoTracking()
                .AnyAsync(t => t.IdTiquete == id);
        }
    }
}