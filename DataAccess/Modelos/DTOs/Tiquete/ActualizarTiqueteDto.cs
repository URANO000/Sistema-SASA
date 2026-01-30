using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class ActualizarTiqueteDto
    {
        public int IdTiquete { get; init; }
        public int IdEstatus { get; init; }
        public int? IdPrioridad { get; init; }
        public int? IdCola { get; init; }
        public int? IdAsignee { get; init; }
        public string? Resolucion { get; init; }
    }
}
