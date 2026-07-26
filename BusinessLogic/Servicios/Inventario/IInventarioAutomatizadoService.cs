using DataAccess.Modelos.DTOs.Inventario;

namespace BusinessLogic.Servicios.Inventario
{
    public interface IInventarioAutomatizadoService
    {
        Task<(bool Ok, string Mensaje, int? Id, bool Actualizado)> RegistrarAsync(
            InventarioAutomatizadoRequestDto request);
    }
}