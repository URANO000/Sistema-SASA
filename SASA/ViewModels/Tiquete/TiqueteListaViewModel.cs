using SASA.ViewModels.Tiquete.Extras;
using System.Collections.Generic;

namespace SASA.ViewModels.Tiquete
{
    public class TiqueteListaViewModel : TiqueteFormViewModel
    {
        public int IdTiquete { get; init; } = default!;
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public string? Resolucion { get; init; }

        //Nombres de cada uno (no todo el obj)
        public required string Estatus { get; init; }
        public string Categoria { get; init; }

        public string? ReportedBy { get; init; }
        public string? Departamento { get; init; }
        public string? Asignee { get; init; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
