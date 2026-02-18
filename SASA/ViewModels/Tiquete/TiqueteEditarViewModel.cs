using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Tiquete
{
    public class TiqueteEditarViewModel : TiqueteFormViewModel
    {
        //Va a estar hidden en la vista, pero es necesario para identificar el tiquete a editar
        [Required]
        public int IdTiquete { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        public required string Asunto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public required int IdCategoria { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public required int IdPrioridad { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        public required int IdEstatus { get; set; }
        public string? Resolucion { get; set; }
        public string? IdAsignee { get; set; }

    }
}
