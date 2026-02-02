using DataAccess.Modelos.Entidades.Inventario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Modelos.Entidades.Inventario
{
    public class ActivoInventario
    {
        public int IdActivo { get; set; }
        public string NumeroActivo { get; set; } = string.Empty;

        public string? NombreMaquina { get; set; }
        public string? UsuarioActualId { get; set; }
        public string? UsuarioAnteriorId { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? SerieServicio { get; set; }
        public string? DireccionMAC { get; set; }
        public string? SistemaOperativo { get; set; }
        public string? ClaveLicencia { get; set; }

        public int IdTipoActivo { get; set; }
        public int IdEstadoActivo { get; set; }
        public int? IdTipoLicencia { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public TipoActivoInventario? TipoActivo { get; set; }
        public EstadoActivoInventario? EstadoActivo { get; set; }
        public TipoLicenciaInventario? TipoLicencia { get; set; }
    }
}

