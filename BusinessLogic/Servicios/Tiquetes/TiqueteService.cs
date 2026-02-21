using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Tiquete.Usuario_Ver;
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
        public TiqueteService(ITiqueteRepository tiqueteRepository, ICategoriaRepository categoriaRepository)
        {
            _tiqueteRepository = tiqueteRepository;
            _categoriaRepository = categoriaRepository;
        }
        //Implementación de los métodos para el servicio de Tiquete

        public async Task<PagedResult<ListaTiqueteDTO>> ObtenerTiquetesAsync(TiqueteFiltroDto filtro)
        {
            return await _tiqueteRepository.ObtenerTiquetesAsync(filtro);
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
        public async Task<ListaTiqueteDTO?> ObtenerTiquetePorIdReadAsync(int id)
        {
            var dto = await _tiqueteRepository.ObtenerTiquetePorIdReadAsync(id);

            //Validar
            if (dto == null)
            {
                return null;
            }

            //Retornar dto
            return new ListaTiqueteDTO
            {
                IdTiquete = dto.IdTiquete,
                Asunto = dto.Asunto,
                Descripcion = dto.Descripcion,
                Resolucion = dto.Resolucion,
                Estatus = dto.Estatus,
                Categoria = dto.Categoria,
                Asignee = dto.Asignee,
                ReportedBy = dto.ReportedBy,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
            };
        }

        public async Task<TiquetePorIdDto?> ObtenerTiquetePorIdAsync(int id)
        {
            return await _tiqueteRepository.ObtenerTiquetePorIdAsync(id);
        }

        //--------------------Creación de tiquetes para el administrador-----------------------------------
        public async Task<int> AgregarTiqueteAsync(CrearTiqueteAdminDto dto, string currentUserId)
        {
            //Validaciones básicas
            if (string.IsNullOrWhiteSpace(currentUserId))
                throw new UnauthorizedAccessException();
            //Validar que la categoría exista
            var categoriaExiste = await _categoriaRepository.ExisteAsync(dto.IdCategoria);
            if (!categoriaExiste)
            {
                throw new ArgumentException("Categoria no existe.");
            }


            //Si todo esto es correcto, entonces se puede mapear el dto a la entidad
            var tiquete = new Tiquete
            {
                Asunto = dto.Asunto.Trim(),
                Descripcion = dto.Descripcion.Trim(),
                IdCategoria = dto.IdCategoria,
                IdAsignee = dto.IdAsignee,
                IdReportedBy = currentUserId,
                IdEstatus = (int)TiqueteEstatus.Creado,
                CreatedAt = DateTime.Now
            };


            var tiqueteCreado = await _tiqueteRepository.AgregarTiqueteAsync(tiquete);
            return tiqueteCreado.IdTiquete;
        }

        //--------------------Creación de tiquetes para el usuario común-----------------------------------
        public Task<int> AgregarTiqueteUsuarioAsync(CrearTiqueteUsuarioDto tiquete, string currentUserId)
        {
            throw new NotImplementedException();
        }

        //--------------------Actualizar tiquetes para el administrador-----------------------------------

        public async Task ActualizarTiqueteAsync(EditarTiqueteDto dto)
        {
            //Validacion 1: El usuario debe estar autenticado para actualizar un tiquete
            //Puede fallar si se prueba con http y no https por los cookies
            if (string.IsNullOrWhiteSpace(dto.UpdatedBy))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }

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
                throw new InvalidOperationException("No se puede actualizar un tiquete cerrado.");
            }

            //Validacion 4: Si el estatus del tiquete se mueve a cerrado, entonces se debe validar que el campo de resolución no esté vacío
            if (dto.IdEstatus == (int)TiqueteEstatus.Cancelado && string.IsNullOrWhiteSpace(dto.Resolucion))
            {
                throw new ArgumentException("La resolución es requerida para cerrar un tiquete.");
            }


            //Si el tiquete existe, se actualizan los campos
            tiqueteActual.Asunto = dto.Asunto.Trim();
            tiqueteActual.Descripcion = dto.Descripcion.Trim();
            tiqueteActual.IdCategoria = dto.IdCategoria;
            tiqueteActual.IdAsignee = dto.IdAsignee;
            tiqueteActual.IdEstatus = dto.IdEstatus;
            tiqueteActual.Resolucion = dto.Resolucion?.Trim(); //Puede no tener nada
            tiqueteActual.UpdatedAt = DateTime.Now;
            tiqueteActual.UpdatedBy = dto.UpdatedBy; //En el controller se debe pasar el id del usuario autenticado

            //Persistencia de datos -> Guardar cambios
            await _tiqueteRepository.ActualizarTiqueteAsync(tiqueteActual);
        }

    }
}