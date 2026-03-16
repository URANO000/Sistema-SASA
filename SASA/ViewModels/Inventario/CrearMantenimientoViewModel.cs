using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Inventario
{
    public class CrearMantenimientoViewModel
    {
        public int IdMantenimiento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un activo.")]
        public int IdActivo { get; set; }

        [Required(ErrorMessage = "La fecha de mantenimiento es requerida.")]
        public DateTime FechaMantenimiento { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de mantenimiento.")]
        public string TipoMantenimiento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public string Estado { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public SelectList? Activos { get; set; }
    }
}
