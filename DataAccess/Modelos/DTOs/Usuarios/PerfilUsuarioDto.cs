namespace DataAccess.Modelos.DTOs.Usuarios
{
    public class PerfilUsuarioDto
    {
        public string Id { get; set; } = string.Empty;

        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }

        public string? CorreoEmpresa { get; set; }

        public string? Departamento { get; set; }
        public string? Puesto { get; set; }

        public bool Estado { get; set; }

        public string? Rol { get; set; }
    }
}