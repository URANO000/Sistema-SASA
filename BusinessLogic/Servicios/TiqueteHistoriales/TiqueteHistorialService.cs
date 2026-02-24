using DataAccess.Modelos.DTOs.TiqueteHistorial;
using DataAccess.Modelos.Entidades.ModTiquete;
using DataAccess.Modelos.Enums;
using DataAccess.Repositorios.TiqueteHistoriales;

namespace BusinessLogic.Servicios.TiqueteHistoriales
{
    public class TiqueteHistorialService : ITiqueteHistorialService
    {
        private readonly ITiqueteHistorialRepository _repository;
        public TiqueteHistorialService(ITiqueteHistorialRepository repository)
        {
            _repository = repository;
        }

        //Centralizar creación de entidad en este método privado
        private async Task RegistrarEventoAsync(
        int idTiquete,
        TipoEventoTiquete tipoEvento,
        string? campoAfectado,
        string? valorAnterior,
        string? valorNuevo,
        string? descripcion,
        string performedBy)
        {
            var entity = new TiqueteHistorial
            {
                IdTiquete = idTiquete,
                TipoEvento = tipoEvento,
                CampoAfectado = campoAfectado,
                ValorAnterior = valorAnterior,
                ValorNuevo = valorNuevo,
                DescripcionEvento = descripcion,
                PerformedAt = DateTime.UtcNow,
                PerformedBy = performedBy
            };

            await _repository.AgregarTiqueteHistorialAsync(entity);
        }


        //Registrar que un tiquete fue creado (y por quién)
        public async Task RegistrarTiqueteCreadoAsync(int idTiquete, string performedBy)
        {
            await RegistrarEventoAsync(
                idTiquete,
                TipoEventoTiquete.TiqueteCreado,
                null,
                null,
                null,
                "Tiquete creado",
                performedBy);
        }

        //Registrar cambio de estado del tiquete (de qué a qué)
        public async Task RegistrarCambioEstadoAsync(
            int idTiquete,
            string estadoAnterior,
            string estadoNuevo,
            string performedBy)
        {
            await RegistrarEventoAsync(
                idTiquete,
                TipoEventoTiquete.CambioDeEstatus,
                "Estado",
                estadoAnterior,
                estadoNuevo,
                $"Estado cambiado de {estadoAnterior} a {estadoNuevo}",
                performedBy);
        }

        //Registrar cuando se ha asignado (externamente) el tiquete 
        public async Task RegistrarAsignacionAsync(
            int idTiquete,
            string usuarioAsignado,
            string performedBy)
        {
            await RegistrarEventoAsync(
                idTiquete,
                TipoEventoTiquete.Asignado,
                "IdAsignee",
                null,
                usuarioAsignado,
                $"Tiquete asignado a {usuarioAsignado}",
                performedBy);
        }

        //Registrar algún cambio de categoría
        public async Task RegistrarCambioCategoriaAsync(
            int idTiquete,
            string categoriaAnterior,
            string categoriaNueva,
            string performedBy)
        {
            await RegistrarEventoAsync(
                idTiquete,
                TipoEventoTiquete.CambioDeCategoria,
                "IdCategoria",
                categoriaAnterior,
                categoriaNueva,
                $"Categoría cambiada de {categoriaAnterior} a {categoriaNueva}",
                performedBy);
        }

        //Registrar creación de avance
        public async Task RegistrarAvanceAsync(
            int idTiquete,
            string descripcionAvance,
            string performedBy)
        {
            await RegistrarEventoAsync(
                idTiquete,
                TipoEventoTiquete.AvanceAgregado,
                null,
                null,
                null,
                descripcionAvance,
                performedBy);
        }

        public async Task<List<TiqueteHistorialPorIdDto>> GetByTiqueteIdAsync(int idTiquete)
        {
            return await _repository.GetHistorialByTiqueteIdAsync(idTiquete);
        }
    }
}
