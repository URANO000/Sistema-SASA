namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class TiquetePorIdDto
    {
        public int IdTiquete { get; set; }
        public string? Asunto { get; set; } = default!;
        public string? Descripcion { get; set; } = default!;
        public int IdCategoria { get; set; }
        public int IdSubCategoria { get; set; }
        public string NombrePrioridad { get; set; }
        public int IdEstatus { get; set; }
        public string? IdAsignee { get; set; }
        public string? Resolucion { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string? ReportedByEmail { get; set; }
        public string? ReportedByNombre { get; set; }
        public string? EstatusNombre { get; set; }
    }
}
