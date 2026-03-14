namespace DataAccess.Modelos.DTOs.Tiquete.Colas
{
    public class ColaTiqueteDto
    {
        public int IdTiquete { get; init; }
        public string? Asunto { get; init; }
        public string? Descripcion { get; set; }

        public int? OrdenCola { get; set; }

        public string? Estatus { get; set; }
        public string? Categoria { get; set; }
        public string? SubCategoria { get; set; }
        public string? Prioridad { get; set; }
        public string? ReportedBy { get; set; }
        public string? Departamento { get; set; }
        public string? Asignee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
