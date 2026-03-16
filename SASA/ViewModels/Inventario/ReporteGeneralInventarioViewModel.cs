using DataAccess.Modelos.DTOs.Inventario;

namespace SASA.ViewModels.Inventario
{
    public class ReporteGeneralInventarioViewModel
    {
        public int TotalActivos { get; set; }
        public int ActivosActivos { get; set; }
        public int EnMantenimiento { get; set; }

        public List<ActivoTelefonoInventarioListItemDto> Items { get; set; } = new();
    }
}