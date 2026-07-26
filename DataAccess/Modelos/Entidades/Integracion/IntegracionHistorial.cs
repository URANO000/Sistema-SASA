using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Modelos.Entidades.Integracion
{
    [Table("IntegracionHistorial")]
    public class IntegracionHistorial
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("TipoProceso")]
        public string TipoProceso { get; set; } = "Importacion"; // Importacion / Exportacion
        [Column("Modulo")]
        public string Modulo { get; set; } = "Inventario";       // por si luego hay más

        [Column("NombreArchivo")]
        public string NombreArchivo { get; set; } = string.Empty;
        [Column("TipoMime", TypeName = "varchar(100)")]
        public string TipoMime { get; set; } = string.Empty;
        [Column("PesoArchivo", TypeName = "BIGINT")]
        public long PesoArchivo { get; set; }
        [Column("Archivo", TypeName = "varbinary(max)")]
        public byte[]? Archivo { get; set; } //Base64  --> Varbinary

        [NotMapped]
        public IFormFile? ArchivoFile { get; set; } = null!;


        [Column("RutaArchivo")]
        public string RutaArchivo { get; set; } = string.Empty;

        [Column("Fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Column("TotalFilas")]
        public int TotalFilas { get; set; }
        [Column("FilasValidas")]
        public int FilasValidas { get; set; }
        [Column("FilasConError")]
        public int FilasConError { get; set; }

        [Column("Estado")]
        public string Estado { get; set; } = "Cargado"; // Cargado / Validado / Importado / Fallido
        [Column("DetalleError")]
        public string? DetalleError { get; set; }

        [Column("UsuarioEjecutorId", TypeName = "varchar(50)")]
        public string? UsuarioEjecutorId { get; set; } // opcional
    }
}