using System.ComponentModel.DataAnnotations;

namespace DataAccess.Modelos.DTOs.Usuarios
{
    public class ListaUsuarioDto
    {
        public string? Id { get; init; }
        public required string PrimerNombre { get; init; }
        public string? SegundoNombre { get; init; }
        public required string PrimerApellido { get; init; }
        public string? SegundoApellido { get; init; }
        public string? Departamento { get; init; }
        public string? Puesto { get; init; }
        public string? CorreoEmpresa { get; init; }
        public bool Estado { get; init; }
        public DateTime? CreatedAt { get; init; }
        public string? CreatedById { get; init; }

        [Required]
        //Roles asignados al usuario
        public IReadOnlyCollection<string>? Roles { get; set; }

    }
}
