using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SASA.ViewModels.Usuario
{
    public sealed class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El primer nombre es obligatorio.")]
        public required string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        public required string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        [Required(ErrorMessage = "El correo empresarial es obligatorio.")]
        [EmailAddress]
        public required string CorreoEmpresa { get; set; }
        [Required(ErrorMessage = "El departamento es obligatorio.")]
        public required string Departamento { get; set; }
        [Required(ErrorMessage = "El puesto es obligatorio.")]
        public required string Puesto { get; set; }

        //POST para seleccionar rol
        [Required]
        public required string Rol { get; set; }

        //GET para roles disponibles
        public IReadOnlyList<SelectListItem>? RolesDisponibles { get; set; }
    }
}
