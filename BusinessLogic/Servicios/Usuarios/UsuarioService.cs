using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;
using DataAccess.Modelos.Entidades;
using DataAccess.Repositorios.Usuarios;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Servicios.Usuarios

{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsuarioService(IUsuarioRepository usuarioRepository, UserManager<ApplicationUser> userManager)
        {
            _usuarioRepository = usuarioRepository;  
            _userManager = userManager;
        }

        //Implementación de los métodos para el servicio de Usuario

        //Obtener todos los usuarios
        //public async Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync()
        //{
        //    return await _usuarioRepository.ObtenerUsuariosAsync();
        //}

        public async Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync()
        {
            var usuarios = await _usuarioRepository.ObtenerUsuariosAsync();

            var resultado = new List<ListaUsuarioDto>(usuarios.Count);

            //Lógica para sacar rol
            foreach (var dto in usuarios)
            {
                var user = await _userManager.FindByIdAsync(dto.Id!);
                var roles = user is not null
                    ? await _userManager.GetRolesAsync(user)
                    : Array.Empty<string>();

                resultado.Add(new ListaUsuarioDto
                {
                    Id = dto.Id,
                    PrimerNombre = dto.PrimerNombre,
                    SegundoNombre = dto.SegundoNombre,
                    PrimerApellido = dto.PrimerApellido,
                    SegundoApellido = dto.SegundoApellido,
                    Departamento = dto.Departamento,
                    Puesto = dto.Puesto,
                    CorreoEmpresa = dto.CorreoEmpresa,
                    Estado = dto.Estado,
                    Roles = roles is IReadOnlyCollection<string> readOnlyRoles ? readOnlyRoles : roles.ToList()
                });
            }

            return resultado;
        }

        //Obtener usuario por ID
        public async Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id)
        {
            var dto = await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);

            if(dto == null)
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return dto;
            }

            var roles = await _userManager.GetRolesAsync(user);

            //Aprendí un término nuevo, esto se llama rehidratación
            return new ListaUsuarioDto
            {
                Id = dto.Id,
                PrimerNombre = dto.PrimerNombre,
                SegundoNombre = dto.SegundoNombre,
                PrimerApellido = dto.PrimerApellido,
                SegundoApellido = dto.SegundoApellido,
                Departamento = dto.Departamento,
                Puesto = dto.Puesto,
                CorreoEmpresa = dto.CorreoEmpresa,
                Estado = dto.Estado,
                Roles = roles is IReadOnlyCollection<string> readOnlyRoles ? readOnlyRoles : roles.ToList()
            };
        }

        //Creación de usuarios
        public async Task AgregarUsuarioAsync(CrearUsuarioDto dto)
        {
            var usuario = new ApplicationUser
            {
                UserName = dto.CorreoEmpresa,
                Email = dto.CorreoEmpresa,
                CorreoEmpresa = dto.CorreoEmpresa,

                PrimerNombre = dto.PrimerNombre,
                SegundoNombre = dto.SegundoNombre,
                PrimerApellido = dto.PrimerApellido,
                SegundoApellido = dto.SegundoApellido,

                Departamento = dto.Departamento,
                Puesto = dto.Puesto,

                Estado = true //Por defecto, el usuario se crea como activo

            };

            var resultado = await _userManager.CreateAsync(usuario);

            if (!resultado.Succeeded)
            {
                throw new Exception("Error al crear usuario: " + string.Join(", ", resultado.Errors.Select(e => e.Description)));
            };

            //Manejo de rol único (ahora dto.Rol)
            if (!string.IsNullOrWhiteSpace(dto.Rol))
            {
                var resultadoRol = await _userManager.AddToRoleAsync(usuario, dto.Rol);

                if(!resultadoRol.Succeeded)
                {
                    await _userManager.DeleteAsync(usuario);
                    throw new InvalidOperationException("Error asignando rol al usuario");
                }
            }
        }

        public Task<ApplicationUser?> ActualizarUsuarioAsync(string id, ApplicationUser usuario)
        {
            throw new NotImplementedException();
        }

        public Task DesactivarUsuarioAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
