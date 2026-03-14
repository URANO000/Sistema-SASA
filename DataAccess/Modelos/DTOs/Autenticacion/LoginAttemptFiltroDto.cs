namespace DataAccess.Modelos.DTOs.Autenticacion
{
    public class LoginAttemptFiltroDto
    {
        public string? EmailIngresado { get; set; }
        public bool? Exitoso { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}