using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.Entidades.ModTiquete;

namespace BusinessLogic.Servicios.TiqueteHistoriales
{
    public interface ITiqueteHistorialService
    {
        //En lugar de un solo agregar historial tiquete
        Task RegistrarTiqueteCreadoAsync(int idTiquete, string performedBy);

        Task RegistrarCambioEstadoAsync(
            int idTiquete,
            string estadoAnterior,
            string estadoNuevo,
            string performedBy);

        Task RegistrarAsignacionAsync(
            int idTiquete,
            string usuarioAsignado,
            string performedBy);

        Task RegistrarCambioCategoriaAsync(
            int idTiquete,
            string categoriaAnterior,
            string categoriaNueva,
            string performedBy);

        Task RegistrarAvanceAsync(
            int idTiquete,
            string descripcionAvance,
            string performedBy);
        Task<List<TiqueteHistorialPorIdDto>> GetByTiqueteIdAsync(int idTiquete);
        //Este servicio es para mostrar el historial por Id de tiquete (para detalle de tiquete)
    }
}
