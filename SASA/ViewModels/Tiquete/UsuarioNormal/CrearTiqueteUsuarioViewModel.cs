using SASA.ViewModels.Tiquete.Extras;
using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Tiquete.UsuarioNormal
{
    public class CrearTiqueteUsuarioViewModel : TiqueteFormViewModel
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        public required string Asunto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public required int Categoria { get; set; }

    }
}
