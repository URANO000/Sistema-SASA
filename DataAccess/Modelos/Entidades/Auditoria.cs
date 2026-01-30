using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Modelos.Entidades
{
    [Table("Auditoria")]
    public class Auditoria
    {
        [Key]
        [Column("idAudit")]
        public int idAudit { get; set; }
        [Column("fecha")]
        public DateOnly Fecha { get; set; }
        [Column("hora")]
        public TimeSpan Hora { get; set; }
        [Column("usuario")]
        public string Usuario { get; set; }

        [Column("tabla")]
        public string Tabla { get; set; }

        [Column("accion")]
        public string Accion { get; set; }
    }
}
