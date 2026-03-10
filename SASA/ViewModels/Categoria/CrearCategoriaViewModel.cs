using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Categoria
{
    public class CrearCategoriaViewModel
    {
        [Required(ErrorMessage = "El nombre de la categoría es requerido.")]
        [StringLength(100)]
        [Display(Name = "Nombre de la categoría")]
        public string NombreCategoria { get; set; } = string.Empty;
    }
}