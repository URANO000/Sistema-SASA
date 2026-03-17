namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class EditarTiqueteDto
    {
        public required int IdTiquete { get; init; }
        public int IdCategoria { get; init; }
        public int IdSubCategoria { get; init; }
        public int IdEstatus { get; init; }
        public decimal OrdenCola { get; init; }
        public string? Resolucion { get; init; }
    }
}
