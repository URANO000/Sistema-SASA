using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.Categoria
{
    public class CrearSubCategoriaViewModel
    {
        [Required(ErrorMessage = "La categoría es requerida.")]
        [Display(Name = "Categoría")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "La prioridad es requerida.")]
        [Display(Name = "Prioridad")]
        public int IdPrioridad { get; set; }

        [Required(ErrorMessage = "El nombre de la subcategoría es requerido.")]
        [StringLength(100)]
        [Display(Name = "Nombre de la subcategoría")]
        public string NombreSubCategoria { get; set; } = string.Empty;

        public List<SelectListItem> Categorias { get; set; } = new();
        public List<SelectListItem> Prioridades { get; set; } = new();
    }
}