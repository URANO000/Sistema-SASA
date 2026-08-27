using System.ComponentModel.DataAnnotations;

namespace DataAccess.Modelos.Enums
{
    public enum TipoEventoTiquete
    {
        [Display(Name = "Tiquete Creado")]
        TiqueteCreado = 1,
        [Display(Name = "Cambio de Estatus")]
        CambioDeEstatus = 2,
        [Display(Name = "Asignado")]
        Asignado = 3,
        [Display(Name = "Cambio de Categoría")]
        CambioDeCategoria = 4,
        [Display(Name = "Avance Agregado")]
        AvanceAgregado = 5
    }
}
