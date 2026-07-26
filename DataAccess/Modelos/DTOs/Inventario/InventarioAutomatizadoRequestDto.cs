using System.ComponentModel.DataAnnotations;

namespace DataAccess.Modelos.DTOs.Inventario
{
    public class InventarioAutomatizadoRequestDto
    {
        [StringLength(
            100,
            ErrorMessage = "El nombre del usuario no puede superar los 100 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del equipo es requerido.")]
        [StringLength(
            100,
            ErrorMessage = "El nombre del equipo no puede superar los 100 caracteres.")]
        public string NombreEquipo { get; set; } = string.Empty;

        [StringLength(
            100,
            ErrorMessage = "La marca no puede superar los 100 caracteres.")]
        public string Marca { get; set; } = string.Empty;

        [StringLength(
            150,
            ErrorMessage = "El modelo no puede superar los 150 caracteres.")]
        public string Modelo { get; set; } = string.Empty;

        [StringLength(
            150,
            ErrorMessage = "El número de serie no puede superar los 150 caracteres.")]
        public string SerialPC { get; set; } = string.Empty;

        [StringLength(
            200,
            ErrorMessage = "El modelo del procesador no puede superar los 200 caracteres.")]
        public string ModeloProcesador { get; set; } = string.Empty;

        [StringLength(
            50,
            ErrorMessage = "La velocidad del procesador no puede superar los 50 caracteres.")]
        public string Mhz { get; set; } = string.Empty;

        // Coincide con la variable "so" del ejecutable.
        [StringLength(
            150,
            ErrorMessage = "El sistema operativo no puede superar los 150 caracteres.")]
        public string So { get; set; } = string.Empty;

        [StringLength(
            100,
            ErrorMessage = "El número de compilación no puede superar los 100 caracteres.")]
        public string BuildNumber { get; set; } = string.Empty;

        [Range(0, 4096, ErrorMessage = "La cantidad total de RAM no es válida.")]
        public double TotalRam { get; set; }

        [Range(0, 64, ErrorMessage = "La cantidad de ranuras de RAM no es válida.")]
        public int SlotsRam { get; set; }

        public string SlotDetalle { get; set; } = string.Empty;

        [StringLength(
            200,
            ErrorMessage = "El modelo de almacenamiento no puede superar los 200 caracteres.")]
        public string ModeloAlmacenamiento { get; set; } = string.Empty;

        public string AlmacenamientoDetalle { get; set; } = string.Empty;

        public string Redes { get; set; } = string.Empty;

        public string RedDetalle { get; set; } = string.Empty;

        [Range(0, 20, ErrorMessage = "La cantidad de monitores no es válida.")]
        public int CantMonitores { get; set; }

        public string Monitores { get; set; } = string.Empty;

        public string MonitorDetalle { get; set; } = string.Empty;

        public bool Tiene7 { get; set; }

        public bool Tiene6 { get; set; }

        public bool TieneKonica { get; set; }
    }
}