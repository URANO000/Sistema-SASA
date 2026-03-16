using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.DTOs.Usuarios.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositorios.Usuarios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        //Referencia al contexto de la base de datos
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Implementación de los métodos del repositorio de usuarios
        public async Task<PagedResult<ListaUsuarioDto>> ObtenerUsuariosAsync(UsuarioFiltroDto filtro)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            //Filtrar si el searchbar no está vacío
            if (!string.IsNullOrWhiteSpace(filtro.Search))
            {
                var search = filtro.Search.Trim();

                query = query.Where(u =>
                    (u.PrimerNombre + " " +
                     u.SegundoNombre + " " +
                     u.PrimerApellido + " " +
                     u.SegundoApellido)
                    .Contains(search)
                    || u.CorreoEmpresa.Contains(search));
            }

            //Si el filtro de Departamento no es vacío
            if (!string.IsNullOrWhiteSpace(filtro.Departamento))
            {
                query = query.Where(u => u.Departamento == filtro.Departamento);
            }

            //Si el filtro de Estado mo es vacío
            if (filtro.Estado != null)
            {
                query = query.Where(u => u.Estado == filtro.Estado);
            }

            var totalRecords = await query.CountAsync();

            var items = await query
                .OrderByDescending(u => u.CreatedAt)

                .Skip((filtro.PageNumber - 1) * filtro.PageSize)

                .Take(filtro.PageSize)
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado,
                    CreatedAt = u.CreatedAt,
                    CreatedById = u.CreatedById

                })
                .ToListAsync();

            //Retrieving de roles
            var userIds = items.Select(x => x.Id).ToList();
            //Pensar en un SQL query
            var rolesData = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new { ur.UserId, r.Name }
                ).ToListAsync();

            var rolesLookup = rolesData
                .GroupBy(x => x.UserId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Name!).ToList());

            foreach (var user in items)
            {
                if (user.Id != null && rolesLookup.ContainsKey(user.Id))
                    user.Roles = rolesLookup[user.Id];
            }

            return new PagedResult<ListaUsuarioDto>
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filtro.PageSize)
            };

        }

        //Para reportes ------ Lista de todos los usuarios ------------------------------
        public async Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosReporteAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado
                })
                .ToListAsync();
        }

        public async Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new ListaUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Estado = u.Estado
                })
                .FirstOrDefaultAsync();
        }

        //Obtener perfil de usuario por id, para mostrar en el perfil del usuario
        public async Task<PerfilUsuarioDto?> ObtenerPerfilPorIdAsync(string id)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new PerfilUsuarioDto
                {
                    Id = u.Id,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    CorreoEmpresa = u.CorreoEmpresa,
                    Departamento = u.Departamento,
                    Puesto = u.Puesto,
                    Estado = u.Estado
                })
                .FirstOrDefaultAsync();
        }

        public async Task ActualizarUsuarioAsync(ApplicationUser usuario)
        {
            _context.Users.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarUsuario(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = false; //bit 0
                await _context.SaveChangesAsync();
            }
        }

        public async Task ActivarUsuario(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = true; //bit 1
                await _context.SaveChangesAsync();
            }
        }

        //Obtener usuarios de TI
        public async Task<IReadOnlyList<UsuarioTIDropdownDto?>> ObtenerUsuariosTIAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Departamento == "Tecnologías de Información" && u.Estado != false)
                .OrderBy(u => u.PrimerNombre)
                .Select(u => new UsuarioTIDropdownDto
                {
                    Id = u.Id,
                    UserName = u.UserName
                })
                .ToListAsync();
        }

    }
}