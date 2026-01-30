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

    }
}
