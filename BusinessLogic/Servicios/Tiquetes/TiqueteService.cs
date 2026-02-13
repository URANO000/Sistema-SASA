using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;
using DataAccess.Modelos.Enums;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Colas;
using DataAccess.Repositorios.Tiquetes;

namespace BusinessLogic.Servicios.Tiquetes
{
    public class TiqueteService : ITiqueteService
    {
        //Repositorio de Tiquete
        private readonly ITiqueteRepository _tiqueteRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IColaRepository _colaRepository;
        public TiqueteService(ITiqueteRepository tiqueteRepository, ICategoriaRepository categoriaRepository, IColaRepository colaRepository)
        {
            _tiqueteRepository = tiqueteRepository;
            _categoriaRepository = categoriaRepository;
            _colaRepository = colaRepository;
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

        public async Task<int> AgregarTiqueteAsync(CrearTiqueteAdminDto dto, string currentUserId)
        {
            //Validaciones básicas
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException();

            //if (string.IsNullOrWhiteSpace(dto.Asunto))
            //    throw new ArgumentException("Asunto requerido");

            //if (string.IsNullOrWhiteSpace(dto.Descripcion))
            //    throw new ArgumentException("Descripción requerida");

            //Validar que la categoría exista
            var categoriaExiste = await _categoriaRepository.ExisteAsync(dto.IdCategoria);
            if (!categoriaExiste)
            {
                throw new ArgumentException("Categoria no existe.");
            }

            //Validar que la cola existe por dicha categoría
            int colaId = dto.IdCola
                ?? await _colaRepository.ObtenerColaPorCategoriaAsync(dto.IdCategoria);

            //Si todo esto es correcto, entonces se puede mapear el dto a la entidad
            var tiquete = new Tiquete
            {
                Asunto = dto.Asunto.Trim(),
                Descripcion = dto.Descripcion.Trim(),
                IdCategoria = dto.IdCategoria,
                IdPrioridad = dto.IdPrioridad,
                IdCola = colaId,
                IdAsignee = dto.IdAsignee,
                IdReportedBy = currentUserId,
                IdEstatus = (int)TiqueteEstatus.Abierto,
                CreatedAt = DateTime.Now
            };


            var tiqueteCreado = await _tiqueteRepository.AgregarTiqueteAsync(tiquete);
            return tiqueteCreado.IdTiquete;
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