using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Modelos.Entidades.Integracion
{
    public class IntegracionHistorial
    {
        public int Id { get; set; }

        public string TipoProceso { get; set; } = "Importacion"; // Importacion / Exportacion
        public string Modulo { get; set; } = "Inventario";       // por si luego hay más

        public string NombreArchivo { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int TotalFilas { get; set; }
        public int FilasValidas { get; set; }
        public int FilasConError { get; set; }

        public string Estado { get; set; } = "Cargado"; // Cargado / Validado / Importado / Fallido
        public string? DetalleError { get; set; }

        public string? UsuarioEjecutorId { get; set; } // opcional
    }
}

