using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.Usuario
{
    public sealed class CrearUsuarioViewModel
    {
        public CrearUsuarioDto Usuario { get; init; } = new CrearUsuarioDto
        {
            PrimerNombre = string.Empty,
            PrimerApellido = string.Empty,
            CorreoEmpresa = string.Empty
        };
        public IReadOnlyList<SelectListItem>? Roles { get; init; }
    }
}
