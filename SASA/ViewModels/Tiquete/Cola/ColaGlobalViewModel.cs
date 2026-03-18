namespace SASA.ViewModels.Tiquete.Cola
{
    public class ColaGlobalViewModel
    {
        public string AssigneeId { get; init; }
        public string AssigneeNombre{ get; init; }

        public List<ColaPersonalViewModel> Colas { get; init; } = new();
    }
}
