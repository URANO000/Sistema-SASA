using System.Collections.Generic;

namespace SASA.ViewModels.Tiquete
{
    public class TiqueteListaViewModel
    {
        public int IdTiquete { get; init; } = default!;
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public string? Resolucion { get; init; }

        //Nombres de cada uno (no todo el obj)
        public required string Estatus { get; init; }
        public string? Prioridad { get; init; }
        public string Categoria { get; init; }
        public string? Cola { get; init; }

        public string? ReportedBy { get; init; }
        public string? Asignee { get; init; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
