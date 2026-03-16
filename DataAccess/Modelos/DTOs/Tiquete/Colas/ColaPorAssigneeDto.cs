namespace DataAccess.Modelos.DTOs.Tiquete.Colas
{
    public class ColaPorAssigneeDto
    {
        public string AssigneeId { get; set; }
        public string AssigneeCorreo { get; set; }

        public List<ColaTiqueteDto> Colas { get; set; }
    }
}
