using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Modelos.Entidades.ModTiquete;

namespace DataAccess.Modelos.Entidades.Inventario
{
    [Table("ActivoInventarioTiquete")]
    public class ActivoInventarioTiquete
    {
        [Key]
        public int IdActivoInventarioTiquete { get; set; }

        public int IdActivo { get; set; }

        public int IdTiquete { get; set; }

        public DateTime FechaAsociacion { get; set; }

        [ForeignKey(nameof(IdActivo))]
        public ActivoInventario? Activo { get; set; }

        [ForeignKey(nameof(IdTiquete))]
        public Tiquete? Tiquete { get; set; }
    }
}