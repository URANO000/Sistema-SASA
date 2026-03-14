

using DataAccess.Modelos.DTOs.Attachments;
using DataAccess.Modelos.DTOs.Avances;
using DataAccess.Modelos.DTOs.TiqueteHistorial;

namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class DetalleTiqueteDto
    {
        public int IdTiquete { get; init; }
        public required string Asunto { get; init; }
        public required string Descripcion { get; set; }
        public string? Resolucion { get; set; }

        public required string Estatus { get; set; }
        public string Categoria { get; set; }
        public string? SubCategoria { get; set; }
        public string Prioridad { get; set; }

        public required string ReportedBy { get; set; }
        public string? Departamento { get; set; }
        public string? Assignee { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //Para detalle, ver los avances
        public List<ListaAvancesDto> Avances { get; init; } = new();
        public List<ListaAttachmentDto> Attachments { get; init; } = new();
        public List<TiqueteHistorialPorIdDto> Historiales { get; init; } = new();
    }
}
