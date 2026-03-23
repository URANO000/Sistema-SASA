using System;

namespace SASA.ViewModels.Home
{
    public class DashboardViewModel
    {
        public int Abiertos { get; set; }
        public int EnProgreso { get; set; }
        public int Resueltos { get; set; }
        public int Cancelados { get; set; }
        public int EnEsperaDelUsuario { get; set; }
        public int Total => Abiertos + EnProgreso + Resueltos + Cancelados + EnEsperaDelUsuario;

        public string? Rol { get; set; }
        public string[] PriorityLabels { get; set; } = Array.Empty<string>();
        public int[] PriorityCounts { get; set; } = Array.Empty<int>();
        public int[] PriorityTicketCounts { get; set; } = Array.Empty<int>();
        public string[] PriorityDisplayLabels { get; set; } = Array.Empty<string>();
        public string[] TrendLabels { get; set; } = Array.Empty<string>();
        public int[] TrendCreados { get; set; } = Array.Empty<int>();
        public int[] TrendResueltos { get; set; } = Array.Empty<int>();
        public int[] TrendEnProgreso { get; set; } = Array.Empty<int>();
        public int[] TrendEspera{ get; set; } = Array.Empty<int>();
        public int[] TrendCancelados { get; set; } = Array.Empty<int>();
    }
}
