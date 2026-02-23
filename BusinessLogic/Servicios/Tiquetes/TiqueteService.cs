using BusinessLogic.Servicios.Avances;
using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;
using DataAccess.Modelos.Enums;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Tiquetes;

namespace BusinessLogic.Servicios.Tiquetes
{
    public class TiqueteService : ITiqueteService
    {
        //Repositorio de Tiquete
        private readonly ITiqueteRepository _tiqueteRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IAvanceService _avanceService;
        public TiqueteService(ITiqueteRepository tiqueteRepository, ICategoriaRepository categoriaRepository, IAvanceService avanceService)
        {
            _tiqueteRepository = tiqueteRepository;
            _categoriaRepository = categoriaRepository;
            _avanceService = avanceService;
        }
        //Implementación de los métodos para el servicio de Tiquete

        public async Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro, string? currentUserId)
        {
            return await _tiqueteRepository.ObtenerTiquetesAsync(filtro, currentUserId);
        }

        public async Task<IReadOnlyList<ListaTiqueteDTO>> ObtenerTiquetesReporteAsync()
        {
            var tiquetes = await _tiqueteRepository.ObtenerTiquetesReporteAsync();

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
                    Categoria = tiquete.Categoria,
                    ReportedBy = tiquete.ReportedBy,
                    Asignee = tiquete.Asignee,
                    CreatedAt = tiquete.CreatedAt,
                    UpdatedAt = tiquete.UpdatedAt
                });
            }

            return resultado;

        }
        public async Task<DetalleTiqueteDto?> ObtenerTiquetePorIdReadAsync(int id)
        {
            var dto = await _tiqueteRepository.ObtenerTiquetePorIdReadAsync(id);

            //Validar
            if (dto == null)
            {
                return null;
            }

            //Guardar todos los avances por tiquete
            var avances = await _avanceService.ListaAvancesPorTiqueteAsync(id);

            //Retornar dto
            return new DetalleTiqueteDto
            {
                IdTiquete = dto.IdTiquete,
                Asunto = dto.Asunto,
                Descripcion = dto.Descripcion,
                Resolucion = dto.Resolucion,
                Estatus = dto.Estatus,
                Categoria = dto.Categoria,
                Asignee = dto.Asignee,
                ReportedBy = dto.ReportedBy,
                Departamento = dto.Departamento,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,

                Avances = avances
            };
        }

        public async Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id)
        {
            return await _tiqueteRepository.ObtenerTiquetePorIdAsync(id);
        }

        //--------------------Creación de tiquetes-----------------------------------
        public async Task<int> AgregarTiqueteAsync(CrearTiqueteDto dto, string currentUserId, bool esAdministrador)
        {
            ValidarUsuarioActual(currentUserId);
            await ValidarCategoriaAsync(dto.IdCategoria);

            //Regla de autorización. Si es administrador entonces puede asignar tiquetes, si es empleado normal, no. 
            if (!esAdministrador && dto.IdAsignee != null)
                throw new UnauthorizedAccessException(
                    "Un usuario normal no puede asignar tiquetes."
                );

            return await CrearTiqueteAsync(
                dto.Asunto,
                dto.Descripcion,
                dto.IdCategoria,
                currentUserId,
                null //Usuario normal no puede asignar
            );
        }

        //--------------------Actualizar tiquetes para el administrador-----------------------------------

        public async Task ActualizarTiqueteAsync(EditarTiqueteDto dto, string currentUserId)
        {
            //Validacion 1: El usuario debe estar autenticado para actualizar un tiquete
            //Puede fallar si se prueba con http y no https por los cookies
            ValidarUsuarioActual(currentUserId);

            var tiqueteActual = await _tiqueteRepository.ObtenerEntidadPorIdAsync(dto.IdTiquete);

            //Validacion 2: El tiquete debe existir para ser actualizado
            //Si el tiquete no existe, se lanza una excepción
            if (tiqueteActual == null)
            {
                throw new KeyNotFoundException("Tiquete no existe.");
            }

            //Validacion 3: Si el estatus del tiquete es cerrado, entonces no se puede actualizar el tiquete
            if (tiqueteActual.IdEstatus == (int)TiqueteEstatus.Cancelado)
            {
                throw new InvalidOperationException("No se puede actualizar un tiquete cancelado.");
            }

            //Validacion 4: Si el estatus del tiquete se mueve a cerrado, entonces se debe validar que el campo de resolución no esté vacío
            if (dto.IdEstatus == (int)TiqueteEstatus.Cancelado && string.IsNullOrWhiteSpace(dto.Resolucion))
            {
                throw new ArgumentException("La resolución es requerida para cerrar un tiquete.");
            }


            //Si el tiquete existe, se actualizan los campos
            tiqueteActual.IdCategoria = dto.IdCategoria;
            tiqueteActual.IdEstatus = dto.IdEstatus;
            tiqueteActual.Resolucion = dto.Resolucion?.Trim(); //Puede no tener nada
            tiqueteActual.UpdatedAt = DateTime.Now;
            tiqueteActual.UpdatedBy = currentUserId; //En el controller se debe pasar el id del usuario autenticado

            //Persistencia de datos -> Guardar cambios
            await _tiqueteRepository.ActualizarTiqueteAsync(tiqueteActual);
        }


    //----------------------------HELPERS (DRY)--------------------------------
        private void ValidarUsuarioActual(string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException("Usuario no autenticado");
        }

        private async Task ValidarCategoriaAsync(int idCategoria)
        {
            var categoriaExiste = await _categoriaRepository.ExisteAsync(idCategoria);
            if (!categoriaExiste)
                throw new ArgumentException("Categoria no existe.");
        }

        private async Task<int> CrearTiqueteAsync(
            string asunto,
            string descripcion,
            int idCategoria,
            string reportedBy,
            string? idAsignee
        )
        {
            var tiquete = new Tiquete
            {
                Asunto = asunto.Trim(),
                Descripcion = descripcion.Trim(),
                IdCategoria = idCategoria,
                IdReportedBy = reportedBy,
                IdAsignee = idAsignee,
                IdEstatus = (int)TiqueteEstatus.Creado,
                CreatedAt = DateTime.Now
            };

            var creado = await _tiqueteRepository.AgregarTiqueteAsync(tiquete);
            return creado.IdTiquete;
        }
    }
}