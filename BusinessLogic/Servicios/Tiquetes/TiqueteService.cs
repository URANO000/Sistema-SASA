using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;
using DataAccess.Repositorios.Tiquetes;

namespace BusinessLogic.Servicios.Tiquetes
{
    public class TiqueteService : ITiqueteService
    {
        //Repositorio de Tiquete
        private readonly ITiqueteRepository _tiqueteRepository;

        public TiqueteService(ITiqueteRepository tiqueteRepository)
        {
            _tiqueteRepository = tiqueteRepository;   
        }
        //Implementación de los métodos para el servicio de Tiquete
        public async Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesAsync()
        {
            var tiquetes = await _tiqueteRepository.ObtenerTiquetesAsync();

            var resultado = new List<ListaTiqueteDTO>(tiquetes.Count);

            //Logica para mapear los tiquetes
            foreach (var tiquete in tiquetes)
            {
                resultado.Add(new ListaTiqueteDTO
                {
                    IdTiquete = tiquete.IdTiquete,
                    Asunto = tiquete.Asunto,
                    Descripcion = tiquete.Descripcion,
                    Resolucion = tiquete.Resolucion,
                    Estatus = tiquete.Estatus,
                    Prioridad = tiquete.Prioridad,
                    Categoria = tiquete.Categoria,
                    Cola = tiquete.Cola,
                    ReportedBy = tiquete.ReportedBy,
                    Asignee = tiquete.Asignee,
                    CreatedAt = tiquete.CreatedAt,
                    UpdatedAt = tiquete.UpdatedAt
                });
            }

            return resultado;

        }
        public async Task<ListaTiqueteDTO?> ObtenerPorTiqueteIdAsync(int id)
        {
            return await _tiqueteRepository.ObtenerTiquetePorIdAsync(id);
        }

        public Task<Tiquete?> AgregarTiqueteAsync(Tiquete tiquete)
        {
            throw new NotImplementedException();
        }

        public Task<Tiquete?> ActualizarTiqueteAsync(int id, Tiquete tiquete)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelarTiquete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
