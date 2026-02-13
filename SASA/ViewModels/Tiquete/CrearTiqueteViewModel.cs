using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.Prioridad;
using DataAccess.Modelos.DTOs.Usuarios;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Tiquete
{
    public class CrearTiqueteViewModel
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        public required string Asunto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public required int Categoria { get; set; }

        [Required(ErrorMessage = "La prioridad es obligatoria.")]
        public int? Prioridad { get; set; }
        public string? IdAsignee { get; set; }

        //Colecciones para dropdowns
        public IEnumerable<SelectListItem>? Categorias { get; set; } = [];
        public IEnumerable<SelectListItem>? Prioridades { get; set; } = [];
        public IEnumerable<SelectListItem>? Asignees { get; set; } = [];  //Por ahora un dropdown, luego se debe cambiar a un autocomplete

    }
}
