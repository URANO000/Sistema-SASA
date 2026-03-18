using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Colas;
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
                    t.Descripcion.Contains(filtro.Search) ||
                    t.Asignee.PrimerNombre.Contains(filtro.Search) ||
                    t.Asignee.PrimerApellido.Contains(filtro.Search)
                    );
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
                var fechaSiguiente = fecha.AddDays(1);

                query = query.Where(t => t.CreatedAt >= fecha && t.CreatedAt < fechaSiguiente);
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
                    ReportedBy = t.ReportedBy != null ? t.ReportedBy.PrimerNombre + " " + t.ReportedBy.PrimerApellido : "Desconocido",
                    Departamento = t.ReportedBy.Departamento,
                    Assignee = t.Asignee != null ? t.Asignee.PrimerNombre + " " + t.Asignee.PrimerApellido : "Sin Asignar",
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
                    SubCategoria = t.SubCategoria != null ? t.SubCategoria.NombreSubCategoria : "Sin subcategoría",

                    ReportedBy = t.ReportedBy != null ? t.ReportedBy.CorreoEmpresa : "Desconocido",
                    Assignee = t.Asignee != null ? t.Asignee.CorreoEmpresa : "Sin asignar",

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
                    SubCategoria = t.SubCategoria != null
                        ? t.SubCategoria.NombreSubCategoria
                        : "Sin SubCategoria",
                    ReportedBy = t.ReportedBy.PrimerNombre + " " + t.ReportedBy.PrimerApellido,
                    Departamento = t.ReportedBy.Departamento,
                    Assignee = t.Asignee != null ? t.Asignee.PrimerNombre + " " + t.Asignee.PrimerApellido : "Sin asignar",
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    Prioridad = t.SubCategoria.Prioridad.NombrePrioridad,
                    DuracionMinutos = t.SubCategoria.Prioridad.DuracionMinutos

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
            IdSubCategoria = t.IdSubCategoria,
            NombrePrioridad = t.SubCategoria.Prioridad.NombrePrioridad,
            IdEstatus = t.IdEstatus,
            IdAsignee = t.IdAsignee,
            Resolucion = t.Resolucion,
            ReportedByEmail = t.ReportedBy.PrimerNombre + t.ReportedBy.PrimerApellido,
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

        //Para asignar masivamente a los tiquetes
        public async Task<List<Tiquete>> ObtenerTiquetesPorIdsAsync(List<int> ids)
        {
            return await _context.Tiquetes
                .Where(t => ids.Contains(t.IdTiquete))
                .ToListAsync();
        }

        public async Task ActualizarAsignacionAsync(List<Tiquete> tiquetes)
        {
            await _context.SaveChangesAsync();
        }




        //-----------------------------------------------------------------------------------------------------
        //---------------------------LÓGICA DE COLAS - Orden de colas y por asignado --------------------------

        //Get All para cola personal
        public async Task<List<ColaTiqueteDto>> GetColaPersonalAsync(string currentUserId)
        {
            var cola = await _context.Tiquetes
                    .AsNoTracking()
                    .Where(t => t.IdAsignee == currentUserId && t.OrdenCola != null)
                    .OrderBy(t => t.OrdenCola)
                    .Select(t => new ColaTiqueteDto
                    {
                        IdTiquete = t.IdTiquete,
                        Asignee = t.Asignee.PrimerNombre + " " + t.Asignee.PrimerApellido,
                        Asunto = t.Asunto,
                        OrdenCola = t.OrdenCola,
                        Categoria = t.Categoria.NombreCategoria,
                        SubCategoria = t.SubCategoria.NombreSubCategoria,
                        Prioridad = t.SubCategoria.Prioridad.NombrePrioridad,
                        DuracionMinutos = t.SubCategoria.Prioridad.DuracionMinutos,
                        Estatus = t.Estatus.NombreEstatus,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

            int posicion = 1; //Para el UI

            foreach (var t in cola)
            {
                t.PosicionCola = posicion++;
            }

            return cola;
        }

        //Get All de todos los de TI - Global
        public async Task<List<ColaPorAssigneeDto>> GetColasGlobalAsync()
        {
            var tiquetes = await _context.Tiquetes
                .AsNoTracking()
                .Where(t => t.IdAsignee != null && t.OrdenCola != null)
                .OrderBy(t => t.IdAsignee)
                .ThenBy(t => t.OrdenCola)
                .Select(t => new {
                    t.IdAsignee,
                    AssigneeNombre = t.Asignee.PrimerNombre + " " + t.Asignee.PrimerApellido,

                    Tiquete = new ColaTiqueteDto
                    {
                        IdTiquete = t.IdTiquete,
                        Asunto = t.Asunto,
                        OrdenCola = t.OrdenCola,
                        Categoria = t.Categoria.NombreCategoria,
                        SubCategoria = t.SubCategoria.NombreSubCategoria,
                        Prioridad = t.SubCategoria.Prioridad.NombrePrioridad,
                        DuracionMinutos = t.SubCategoria.Prioridad.DuracionMinutos
                    }
                })
                .ToListAsync();

            var resultado = tiquetes
                .GroupBy(t => new { t.IdAsignee, t.AssigneeNombre })
                .Select(g =>
                {
                    int posicion = 1; //Para UI

                    var lista = g.Select(x =>
                    {
                        x.Tiquete.PosicionCola = posicion++;
                        return x.Tiquete;
                    }).ToList();

                    return new ColaPorAssigneeDto
                    {
                        AssigneeId = g.Key.IdAsignee,
                        AssigneeNombre = g.Key.AssigneeNombre,
                        Colas = lista
                    };
                })
                .ToList();

            return resultado;
        }

        //Get número que sigue en la cola
        public async Task<decimal> ObtenerSiguienteOrdenColaAsync(string idAssignee)
        {
            var maxOrden = await _context.Tiquetes
                .Where(t => t.IdAsignee == idAssignee && t.OrdenCola != null)
                .MaxAsync(t => (decimal?)t.OrdenCola);

            return (maxOrden ?? 0) + 1000m;
        }

        //Posible para drag & drop
        public decimal CalcularOrdenEntre(decimal ordenAnterior, decimal ordenSiguiente)
        {
            return (ordenAnterior + ordenSiguiente) / 2m;
        }



        //--------------------------------------Para dashboard-------------------------------------
        public async Task<int> ContarTiquetesAsync()
        {
            return await _context.Tiquetes.CountAsync();
        }

        public async Task<List<TiquetesPorEstadoDto>> ObtenerTiquetesPorEstadoAsync()
        {
            return await _context.Tiquetes
                .AsNoTracking()
                .GroupBy(t => t.Estatus.NombreEstatus)
                .Select(g => new TiquetesPorEstadoDto
                {
                    Estado = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<TiquetesPorDiaDto>> ObtenerTiquetesUltimos7DiasAsync()
        {
            var fechaInicio = DateTime.UtcNow.Date.AddDays(-6);

            return await _context.Tiquetes
                .AsNoTracking()
                .Where(t => t.CreatedAt >= fechaInicio)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new TiquetesPorDiaDto
                {
                    Fecha = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(x => x.Fecha)
                .ToListAsync();
        }

        public async Task<double> PromedioResolucion()
        {
            var promedio = await _context.Tiquetes
                .AsNoTracking()
                .Where(t => t.UpdatedAt != null)
                .AverageAsync(t =>
                    EF.Functions.DateDiffMinute(t.CreatedAt, t.UpdatedAt));
            
            return promedio ?? 0.0;
        }



    }
}