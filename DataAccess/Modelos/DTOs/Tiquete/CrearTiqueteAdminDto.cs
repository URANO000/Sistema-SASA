namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class CrearTiqueteAdminDto
    {
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public required int IdCategoria { get; init; }
        public int? IdPrioridad { get; init; }
        public int? IdCola { get; init; }
        public int? IdAsignee { get; init; }
        public int? IdReportedBy { get; init; }
    }
}
