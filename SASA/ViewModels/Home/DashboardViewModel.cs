using System;

namespace SASA.ViewModels.Home
{
    public class DashboardViewModel
    {
        public int Abiertos { get; set; }
        public int EnProgreso { get; set; }
        public int Resueltos { get; set; }
        public int Cancelados { get; set; }
        public int Cerrados { get; set; }
        public int Total => Abiertos + EnProgreso + Resueltos + Cancelados + Cerrados;

        public string? Rol { get; set; }
        public string[] PriorityLabels { get; set; } = Array.Empty<string>();
        public int[] PriorityCounts { get; set; } = Array.Empty<int>();
        public string[] TrendLabels { get; set; } = Array.Empty<string>();
        public int[] TrendCreados { get; set; } = Array.Empty<int>();
        public int[] TrendCerrados { get; set; } = Array.Empty<int>();
        public int[] TrendCancelados { get; set; } = Array.Empty<int>();
    }
}
