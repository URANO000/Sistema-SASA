using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Inventario
{
    public class AsociacionActivoTiqueteViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un activo.")]
        public int? IdActivo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tiquete.")]
        public int? IdTiquete { get; set; }

        public List<SelectListItem> Activos { get; set; } = new();

        public List<SelectListItem> Tiquetes { get; set; } = new();
    }
}