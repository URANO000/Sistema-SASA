using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Tiquete.Usuario_Ver;
using DataAccess.Modelos.DTOs.Wrappers;

namespace BusinessLogic.Servicios.Tiquetes
{
    public interface ITiqueteService
    {
        //Métodos para el servicio de Tiquete

        Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro, string? currentUserId);
        Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesReporteAsync();

        //READ: Nada más para detalle
        Task<ListaTiqueteDTO?> ObtenerTiquetePorIdReadAsync(int id);

        //Para obtener tiquetes por usuario pero para editar (con IDs)
        Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id);

        ////Lista de tiquetes creados por usuario actual (sólo si es un empleado normal)
        //Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesPorCreadorAsync(TiqueteFiltroDto filtro, string idCreador);
        //Creación de tiquetes para el administrador
        Task<int> AgregarTiqueteAsync(CrearTiqueteAdminDto tiquete, string currentUserId);
        //Creación de tiquetes para el cliente
        Task<int> AgregarTiqueteUsuarioAsync(CrearTiqueteUsuarioDto tiquete, string currentUserId);
        //Actualización de tiquetes para el administrador
        Task ActualizarTiqueteAsync(EditarTiqueteDto tiquete);

    }
}