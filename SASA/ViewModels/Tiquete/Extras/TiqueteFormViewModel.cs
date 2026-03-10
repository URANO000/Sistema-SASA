using Microsoft.AspNetCore.Mvc.Rendering;

namespace SASA.ViewModels.Tiquete.Extras
{
    public abstract class TiqueteFormViewModel
    {
        /*Razón: Para evitar la duplicación de código entre los formularios de creación y edición, se creó esta clase base que contiene las propiedades comunes a ambos formularios.
        Esto permite mantener el código más limpio y fácil de mantener, ya que cualquier cambio en estas propiedades solo necesita 
        ser realizado en un lugar */
        public IEnumerable<SelectListItem>? Categorias { get; set; }
        public IEnumerable<SelectListItem>? Asignees { get; set; }
        public IEnumerable<SelectListItem>? Estatuses { get; set; }

        //  3/9/2026  Adición de propiedad para manejar la selección de subcategorías en el formulario
        public IEnumerable<SelectListItem>? SubCategorias { get; set; }
    }
}
