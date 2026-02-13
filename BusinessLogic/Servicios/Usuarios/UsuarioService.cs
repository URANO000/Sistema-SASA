using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Usuarios;
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

            if (dto == null)
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

        //Obtener usuarios por departamento TI
        public async Task<IEnumerable<UsuarioTIDropdownDto?>> ObtenerUsuariosTIAsync()
        {
            return await _usuarioRepository.ObtenerUsuariosTIAsync();
        }



        //Creación de usuarios
        public async Task<ResultadoCreacionUsuarioDto> AgregarUsuarioAsync(CrearUsuarioDto dto)
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

                Estado = true,           // usuario activo por defecto
                EmailConfirmed = false,  // importante para RequireConfirmedEmail
                LockoutEnabled = true    // por si aplica lockout
            };

            // NOTA: Esto crea el usuario SIN contraseña.
            // Si el flujo requiere contraseña temporal, aquí se usaría CreateAsync(usuario, password).
            var resultado = await _userManager.CreateAsync(usuario);

            if (!resultado.Succeeded)
            {
                throw new Exception("Error al crear usuario: " + string.Join(", ", resultado.Errors.Select(e => e.Description)));
            }
            ;


            // Manejo de rol único (dto.Rol)
            if (!string.IsNullOrWhiteSpace(dto.Rol))
            {
                var resultadoRol = await _userManager.AddToRoleAsync(usuario, dto.Rol);

                if (!resultadoRol.Succeeded)
                {
                    await _userManager.DeleteAsync(usuario);
                    throw new InvalidOperationException("Error asignando rol al usuario");
                }
            }

            // Token de confirmación de correo (historia #15)
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);

            // Retornar datos para que el controller construya el link y envíe el correo
            return new ResultadoCreacionUsuarioDto
            {
                UserId = usuario.Id,
                Email = usuario.Email!,
                EmailConfirmationToken = token
            };
        }


        public async Task ActualizarUsuarioAsync(EditarUsuarioDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(dto.Id);

            //Validar que el usuario exista
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");
            }

            //Else, actualizamos los campos
            usuario.PrimerNombre = dto.PrimerNombre;
            usuario.SegundoNombre = dto.SegundoNombre;
            usuario.PrimerApellido = dto.PrimerApellido;
            usuario.SegundoApellido = dto.SegundoApellido;

            usuario.Departamento = dto.Departamento;
            usuario.Puesto = dto.Puesto;

            //Manejo especial de cambio de correo de Identity -- de cuidado
            if (!string.Equals(usuario.Email, dto.CorreoEmpresa, StringComparison.OrdinalIgnoreCase))
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(usuario, dto.CorreoEmpresa);

                var result = await _userManager.ChangeEmailAsync(
                    usuario,
                    dto.CorreoEmpresa,
                    token
                );

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", result.Errors.Select(e => e.Description))
                    );
                }

                //Mantener UserName sincronizado con el correo
                usuario.EmailConfirmed = true;

                var setUserNameResult = await _userManager.SetUserNameAsync(
                    usuario,
                    dto.CorreoEmpresa
                );

                if (!setUserNameResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", setUserNameResult.Errors.Select(e => e.Description))
                    );
                }
            }

            usuario.CorreoEmpresa = dto.CorreoEmpresa;

            //Actualización del usuario
            var resultado = await _userManager.UpdateAsync(usuario);

            //Validar que funciona
            if (!resultado.Succeeded)
            {
                throw new Exception("Error al actualizar usuario: " +
                    string.Join(", ", resultado.Errors.Select(e => e.Description)));
            }

            //Finalmente, se maneja el rol, similar al método de agregar usuario
            var rolesActuales = await _userManager.GetRolesAsync(usuario);

            if (!rolesActuales.Contains(dto.Rol))
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                var resultadoRol = await _userManager.AddToRoleAsync(usuario, dto.Rol);

                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException("Error actualizando el rol del usuario.");
                }
            }


        }

        public async Task DesactivarUsuarioAsync(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id inválido", nameof(id));
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");

            }

            await _usuarioRepository.DesactivarUsuario(id);
        }

        public async Task ActivarUsuarioAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Id inválido", nameof(id));
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");

            }

            await _usuarioRepository.ActivarUsuario(id);
        }
    }
}
