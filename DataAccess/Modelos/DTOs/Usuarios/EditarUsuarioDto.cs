using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Modelos.DTOs.Usuarios
{
    public class EditarUsuarioDto
    {
        [Required]
        public required string Id { get; init; }

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
        public required string Rol { get; init; }
    }
}
