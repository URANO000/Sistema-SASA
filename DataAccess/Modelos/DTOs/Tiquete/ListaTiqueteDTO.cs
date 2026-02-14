namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class ListaTiqueteDTO
    {
        public int IdTiquete { get; init; }
        public required string Asunto { get; set; }
        public required string Descripcion { get; set; }
        public string? Resolucion { get; set; }

        public required string Estatus { get; set; }
        public string? Prioridad { get; set; }
        public string Categoria { get; set; }
        public string? Cola { get; set; }

        public string? ReportedBy { get; set; }
        public string? Asignee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}