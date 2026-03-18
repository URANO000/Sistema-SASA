namespace DataAccess.Modelos.DTOs.Tiquete.Colas
{
    public class ColaPorAssigneeDto
    {
        public string AssigneeId { get; set; }
        public string AssigneeNombre { get; set; }

        public List<ColaTiqueteDto> Colas { get; set; }
    }
}
