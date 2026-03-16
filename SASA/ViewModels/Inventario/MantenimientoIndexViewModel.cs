using DataAccess.Modelos.DTOs.Inventario;

namespace SASA.ViewModels.Inventario
{
    public class MantenimientoIndexViewModel
    {
        public List<MantenimientoActivoListItemDto> Items { get; set; } = new();
    }
}