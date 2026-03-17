namespace SASA.ViewModels.Tiquete.Cola
{
    public class ColaPersonalViewModel
    {
        public int IdTiquete { get; init; }
        public string? Asunto { get; init; }
        public int PosicionCola { get; init; }
        public decimal? OrdenCola { get; init; }

        public string? Estatus { get; init; }
        public string? Categoria { get; init; }
        public string? SubCategoria { get; init; }
        public string? Prioridad { get; init; }
        public string? Asignee { get; init; }

        public DateTime CreatedAt { get; init; }
    }
}
