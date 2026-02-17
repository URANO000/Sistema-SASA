using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Tiquete.Usuario_Ver
{
    public class TiqueteEditarViewModelUsuario
    {
        [Required]
        public int IdTiquete { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        public required string Asunto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Descripcion { get; set; }
    }
}
