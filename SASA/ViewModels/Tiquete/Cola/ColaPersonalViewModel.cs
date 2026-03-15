namespace SASA.ViewModels.Tiquete.Cola
{
    public class ColaPersonalViewModel
    {
        public int IdTiquete { get; init; }
        public string? Asunto { get; init; }
        public int PosicionCola { get; set; }

        public string? Estatus { get; set; }
        public string? Categoria { get; set; }
        public string? SubCategoria { get; set; }
        public string? Prioridad { get; set; }
        public string? Asignee { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
