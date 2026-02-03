namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class ListaTiqueteDTO
    {
        public int IdTiquete { get; init; }
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public string? Resolucion { get; init; }

        public required string Estatus { get; init; }
        public string? Prioridad { get; init; }
        public string Categoria { get; init; }
        public string? Cola { get; init; }

        public string? ReportedBy { get; init; }
        public string? Asignee { get; init; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
