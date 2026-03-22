using SASA.ViewModels.Tiquete.Extras;
using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Tiquete
{
    public class CrearTiqueteViewModel : TiqueteFormViewModel
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        public required string Asunto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public required int Categoria { get; set; }

        [Required(ErrorMessage = "La subcategoría es obligatoria.")]
        public required int IdSubCategoria { get; set; }

        //Relevante sólo para admin
        public string? IdAsignee { get; set; }

        //Opcional: Guardar archivos
        public List<IFormFile>? ArchivosAdjuntos { get; set; }

    }
}
