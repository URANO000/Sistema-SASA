namespace DataAccess.Modelos.DTOs.Tiquete.Colas
{
    public class ReordenarDto
    {
        public int IdTiquete { get; set; }
        public decimal? OrdenAnterior { get; set; }
        public decimal? OrdenSiguiente { get; set; }
    }
}
