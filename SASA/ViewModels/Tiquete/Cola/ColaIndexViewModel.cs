namespace SASA.ViewModels.Tiquete.Cola
{
    public class ColaIndexViewModel
    {
        public string TabActiva { get; set; } = "Cola Personal";

        public List<ColaPersonalViewModel> Personal { get; set; } = new();

    }
}
