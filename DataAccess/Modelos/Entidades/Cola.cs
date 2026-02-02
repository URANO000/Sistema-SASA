
using DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Cola")]
    public class Cola
    {
        [Key]
        [Column("idCola")]
        public int IdCola { get; set; }

        [Column("idCategoria")]
        public int? IdCategoria { get; set; }

        [Required]
        [Column("nombreCola")]
        [StringLength(100)]
        public string NombreCola { get; set; } = null!;

        [Column("descripcionCola")]
        [StringLength(250)]
        public string? DescripcionCola { get; set; }

        [Required]
        [Column("isActive")]
        public bool IsActive { get; set; }

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("createdBy")]
        [StringLength(450)]
        public string CreatedBy { get; set; } = null!;

        //Relaciones
        [ForeignKey(nameof(IdCategoria))]
        public Categoria? Categoria { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public ApplicationUser CreatedByUser { get; set; } = null!;

        public ICollection<Tiquete> Tiquetes { get; set; } = new List<Tiquete>();
    }

}
