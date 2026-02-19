using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Agente_Ver;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Tiquete.Usuario_Ver;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades;

namespace BusinessLogic.Servicios.Tiquetes
{
    public interface ITiqueteService
    {
        //Métodos para el servicio de Tiquete

        Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro);
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesReporteAsync();

        //READ: Nada más para detalle
        Task<ListaTiqueteDTO?> ObtenerTiquetePorIdReadAsync(int id);

        //Para obtener tiquetes por usuario pero para editar (con IDs)
        Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id);

        //Creación de tiquetes para el administrador
        Task<int> AgregarTiqueteAsync(CrearTiqueteAdminDto tiquete, string currentUserId);
        //Creación de tiquetes para el cliente
        Task<int> AgregarTiqueteUsuarioAsync(CrearTiqueteUsuarioDto tiquete, string currentUserId);
        //Actualización de tiquetes para el administrador
        Task ActualizarTiqueteAsync(EditarTiqueteDto tiquete);
        //Actualización de tiquetes para agentes
        Task ActualizarTiqueteAgenteAsync(EditarTiqueteAgenteDto tiquete);
        //Actualización de tiquetes para clientes
        Task ActualizarTiqueteUsuarioAsync(EditarTiqueteUsuarioDto tiquete);
    }
}