namespace DataAccess.Modelos.DTOs.Autenticacion
{
    public class LoginAttemptPagedResultDto
    {
        public IReadOnlyList<LoginAttemptItemDto> Items { get; set; } = new List<LoginAttemptItemDto>();
        public int TotalCount { get; set; }
    }
}