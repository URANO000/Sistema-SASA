namespace DataAccess.Modelos.DTOs.Usuarios
{
    public class CrearUsuarioDto
    {
        public required string PrimerNombre { get; init; }
        public required string PrimerApellido { get; init; }
        public string? Departamento { get; init; }
        public string? Puesto { get; init; }
        public string? CorreoEmpresa { get; init; }

        //Jefe del usuario
    }
}
