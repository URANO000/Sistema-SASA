namespace BusinessLogic.Servicios.Correo
{
    public interface ICorreoNotificacionesService
    {
        Task NotificarConfirmacionPermisoAsync(string toEmail, string nombreEmpleado, string tipoPermiso);
    }
}
