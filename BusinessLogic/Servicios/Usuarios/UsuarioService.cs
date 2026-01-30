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
        public async Task<IReadOnlyList<ListaUsuarioDto>> ObtenerUsuariosAsync()
        {
            return await _usuarioRepository.ObtenerUsuariosAsync();
        }

        //Obtener usuario por ID
        public async Task<ListaUsuarioDto?> ObtenerUsuarioPorIdAsync(string id)
        {
            return await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);
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

            //Manejo de roles
            if (dto.Roles?.Any() == true)
            {
                var resultadoRol = await _userManager.AddToRolesAsync(usuario, dto.Roles);

                if(!resultadoRol.Succeeded)
                {
                    await _userManager.DeleteAsync(usuario);
                    throw new InvalidOperationException("Error asignando roles al usuario");
                }
            };
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
