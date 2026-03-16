
namespace DataAccess.Modelos.DTOs.Prioridad
{
    public class PrioridadDto
    {
        public int IdPrioridad { get; set; }
        public string NombrePrioridad { get; set; } = null!;
        public int DuracionMinutos { get; set; }
    }
}
