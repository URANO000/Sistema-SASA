namespace BusinessLogic.Servicios.Rol
{
    public interface IRolService
    {
        Task<IReadOnlyList<string>> ObtenerRolesAsync();
    }
}
