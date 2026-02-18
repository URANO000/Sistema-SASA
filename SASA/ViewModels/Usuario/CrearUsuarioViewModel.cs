using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Usuario
{
    public sealed class CrearUsuarioViewModel
    {
        [Required]
        public required string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        [Required]
        public required string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        [Required]
        [EmailAddress]
        public required string CorreoEmpresa { get; set; }
        [Required]
        public required string Departamento { get; set; }
        [Required]
        public required string Puesto { get; set; }

        //POST para seleccionar rol
        [Required]
        public required string Rol { get; set; }

        //GET para roles disponibles
        public IReadOnlyList<SelectListItem>? RolesDisponibles { get; set; }
    }
}
