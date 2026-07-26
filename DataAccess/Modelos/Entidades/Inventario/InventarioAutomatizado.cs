using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.Inventario
{
    [Table("InventarioAutomatizado")]
    public class InventarioAutomatizado
    {
        [Key]
        public int IdInventarioAutomatizado { get; set; }

        // Usuario y equipo
        [MaxLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NombreEquipo { get; set; } = string.Empty;

        // Marca, modelo y número de serie
        [MaxLength(100)]
        public string Marca { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Modelo { get; set; } = string.Empty;

        [MaxLength(150)]
        public string SerialPC { get; set; } = string.Empty;

        // Procesador y sistema operativo
        [MaxLength(200)]
        public string ModeloProcesador { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Mhz { get; set; } = string.Empty;

        [MaxLength(150)]
        public string SistemaOperativo { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BuildNumber { get; set; } = string.Empty;

        // RAM
        public double TotalRam { get; set; }

        public int SlotsRam { get; set; }

        public string SlotDetalle { get; set; } = string.Empty;

        // Almacenamiento
        [MaxLength(200)]
        public string ModeloAlmacenamiento { get; set; } = string.Empty;

        public string AlmacenamientoDetalle { get; set; } = string.Empty;

        // Red
        public string Redes { get; set; } = string.Empty;

        public string RedDetalle { get; set; } = string.Empty;

        // Monitores
        public int CantMonitores { get; set; }

        public string Monitores { get; set; } = string.Empty;

        public string MonitorDetalle { get; set; } = string.Empty;

        // Impresoras
        public bool Tiene7 { get; set; }

        public bool Tiene6 { get; set; }

        public bool TieneKonica { get; set; }

        // Auditoría
        public DateTime FechaRegistro { get; set; }

        public DateTime FechaActualizacion { get; set; }
    }
}