using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.Usuario
{
    public sealed class CrearUsuarioViewModel
    {
        public required string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public required string PrimerApellido { get; set; } 
        public string? SegundoApellido { get; set; }
        public required string CorreoEmpresa { get; set; } 

        public required string Departamento { get; set; }
        public required string Puesto { get; set; }

        public List<string> RolesSeleccionados { get; set; } = [];
        public IReadOnlyList<SelectListItem>? RolesDisponibles { get; set; }
    }
}
