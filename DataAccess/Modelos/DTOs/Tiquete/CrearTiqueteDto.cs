using Microsoft.AspNetCore.Http;

namespace DataAccess.Modelos.DTOs.Tiquete
{
    public class CrearTiqueteDto
    {
        public required string Asunto { get; init; }
        public required string Descripcion { get; init; }
        public required int IdCategoria { get; init; }

        public int IdSubCategoria { get; init; }
        public string? IdAsignee { get; init; }

        //Opcional: Archivos adjuntos
        public List<IFormFile>? ArchivoAdjunto { get; set; }
    }
}