namespace DataAccess.Modelos.DTOs.Tiquete.Agente_Ver
{
    public class EditarTiqueteAgenteDto
    {
        public required int IdTiquete { get; init; }
        //public required string Asunto { get; init; }
        //public required string Descripcion { get; init; }
        public int IdEstatus { get; init; }
        public int IdPrioridad { get; init; }
        public string? Resolucion { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public required string UpdatedBy { get; init; }
    }
}
