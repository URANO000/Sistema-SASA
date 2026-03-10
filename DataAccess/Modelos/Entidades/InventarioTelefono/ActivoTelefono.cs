using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades.InventarioTelefono
{
    [Table("ActivoTelefono")]
    public class ActivoTelefono
    {
        [Key]
        [Column("IdTelefono")]
        public int IdActivoTelefono { get; set; }

        [Required]
        [MaxLength(150)]
        public string NombreColaborador { get; set; } = "";

        [MaxLength(150)]
        public string? Departamento { get; set; }

        [MaxLength(100)]
        public string? Operador { get; set; }

        [MaxLength(20)]
        public string? NumeroCelular { get; set; }

        [MaxLength(150)]
        public string? CorreoSistemasAnaliticos { get; set; }

        [MaxLength(100)]
        public string? Modelo { get; set; }

        [MaxLength(50)]
        public string? IMEI { get; set; }

        public bool Cargador { get; set; }

        public bool Auriculares { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }
    }
}