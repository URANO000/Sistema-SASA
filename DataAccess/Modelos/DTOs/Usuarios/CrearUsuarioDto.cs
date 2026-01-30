using System.ComponentModel.DataAnnotations;

namespace DataAccess.Modelos.DTOs.Usuarios
{
    public class CrearUsuarioDto
    {
        [Required]
        public required string PrimerNombre { get; init; }
        public string? SegundoNombre { get; init; }
        [Required]
        public required string PrimerApellido { get; init; }
        public string? SegundoApellido { get; init; }
        public string? Departamento { get; init; }
        public string? Puesto { get; init; }

        [Required]
        [EmailAddress]
        public required string CorreoEmpresa { get; init; }

        [Required]
        //Role asignado al usuario
        public required string Rol { get; init; }
    }
}
