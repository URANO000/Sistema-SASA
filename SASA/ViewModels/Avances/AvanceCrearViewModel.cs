using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Avances
{
    public class AvanceCrearViewModel
    {
        [Required(ErrorMessage = "El avance no puede estar vacío.")]
        [StringLength(2000, ErrorMessage = "El avance es demasiado largo.")]
        public string TextoAvance { get; set; } = string.Empty;
    }
}
