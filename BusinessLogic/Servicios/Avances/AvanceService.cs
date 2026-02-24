using DataAccess.Modelos.DTOs.Avances;
using DataAccess.Modelos.Entidades.ModTiquete;
using DataAccess.Repositorios.Avances;
using DataAccess.Repositorios.Tiquetes;

namespace BusinessLogic.Servicios.Avances
{
    public class AvanceService : IAvanceService
    {
        private readonly IAvanceRepository _repository;
        private readonly ITiqueteRepository _tiqueteRepository;
        public AvanceService(IAvanceRepository repository, ITiqueteRepository tiqueteRepository)
        {
            _repository = repository;
            _tiqueteRepository = tiqueteRepository;
        }
        public async Task<int> AgregarAvanceAsync(CrearAvanceDto dto, string currentUserId, int tiqueteId)
        {
            //Validar usuario actual
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("Usuario no autenticado");

            //Validar que el tiquete existe
            var existeTiquete = await _tiqueteRepository.ExisteTiquete(tiqueteId);
            if(!existeTiquete)
            {
                throw new KeyNotFoundException("El tiquete no existe.");
            }

            //Validar avance
            if (string.IsNullOrWhiteSpace(dto.TextoAvance))
                throw new ArgumentException("El avance no puede estar vacío.");

            //Agregar la información
            var avance = new Avance
            {
                IdTiquete = tiqueteId,
                IdAutor = currentUserId,
                TextoAvance = dto.TextoAvance,
                CreatedAt = DateTime.Now
            };

            //Llamar al repo
            var creado = await _repository.AgregarAvanceAsync(avance);
            return creado.IdAvance;
        }

        public async Task<List<ListaAvancesDto>> ListaAvancesPorTiqueteAsync(int tiqueteId)
        {
            //Primero, existe el tiquete
            var existeTiquete = await _tiqueteRepository.ExisteTiquete(tiqueteId);

            if (!existeTiquete)
            {
                throw new KeyNotFoundException("El tiquete no existe.");
            }

            return await _repository.ListaAvancesPorTiqueteAsync(tiqueteId);
        }
    }
}
