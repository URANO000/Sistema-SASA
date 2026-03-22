using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.DTOs.TiqueteHistorial.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;

namespace BusinessLogic.Servicios.TiqueteHistoriales
{
    public interface ITiqueteHistorialService
    {
        //En lugar de un solo agregar historial tiquete
        Task RegistrarTiqueteCreadoAsync(int idTiquete, string performedBy, bool autoSave = true);

        Task RegistrarCambioEstadoAsync(
            int idTiquete,
            string estadoAnterior,
            string estadoNuevo,
            string performedBy,
            bool autoSave = true);

        Task RegistrarAsignacionAsync(
            int idTiquete,
            string asignadoAnterior,
            string usuarioAsignado,
            string performedBy,
            bool autoSave = true);

        Task RegistrarCambioCategoriaAsync(
            int idTiquete,
            string categoriaAnterior,
            string categoriaNueva,
            string performedBy,
            bool autoSave = true);

        Task RegistrarAvanceAsync(
            int idTiquete,
            string descripcionAvance,
            string performedBy,
            bool autoSave = true);
        Task<List<TiqueteHistorialPorIdDto>> GetByTiqueteIdAsync(int idTiquete);

        Task<PagedResult<ListaTiqueteHistorialDto>> ListarHistorialAsync(TiqueteHistorialFiltroDto filtro);
    }
}
