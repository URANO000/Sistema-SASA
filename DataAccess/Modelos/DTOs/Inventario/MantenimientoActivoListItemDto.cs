using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Modelos.DTOs.Inventario
{
    public class MantenimientoActivoListItemDto
    {
        public int IdMantenimiento { get; set; }
        public int IdActivo { get; set; }
        public string NumeroActivo { get; set; } = string.Empty;
        public string? NombreMaquina { get; set; }
        public DateTime FechaMantenimiento { get; set; }
        public string TipoMantenimiento { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}