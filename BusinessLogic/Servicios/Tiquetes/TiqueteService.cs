using BusinessLogic.Servicios.Avances;
using BusinessLogic.Servicios.TiqueteHistoriales;
using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;
using DataAccess.Modelos.Enums;
using DataAccess.Repositorios.Attachments;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Tiquetes;
using Microsoft.AspNetCore.Http;

namespace BusinessLogic.Servicios.Tiquetes
{
    public class TiqueteService : ITiqueteService
    {
        //Repositorio de Tiquete
        private readonly ITiqueteRepository _tiqueteRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IAvanceService _avanceService;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ITiqueteHistorialService _tiqueteHistorialService;
        public TiqueteService(ITiqueteRepository tiqueteRepository, ICategoriaRepository categoriaRepository,
            IAvanceService avanceService, IAttachmentRepository attachmentRepository,
            ITiqueteHistorialService tiqueteHistorialService)
        {
            _tiqueteRepository = tiqueteRepository;
            _categoriaRepository = categoriaRepository;
            _avanceService = avanceService;
            _attachmentRepository = attachmentRepository;
            _tiqueteHistorialService = tiqueteHistorialService;
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
                    SubCategoria = tiquete.SubCategoria,
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
            //Guardar todos los archivos adjuntos por tiquete
            var attachments = await _attachmentRepository.ListarAttachmentsAsync(id);
            var historiales = await _tiqueteHistorialService.GetByTiqueteIdAsync(id);

            //Retornar dto
            return new DetalleTiqueteDto
            {
                IdTiquete = dto.IdTiquete,
                Asunto = dto.Asunto,
                Descripcion = dto.Descripcion,
                Resolucion = dto.Resolucion,
                Estatus = dto.Estatus,
                Categoria = dto.Categoria,
                SubCategoria = dto.SubCategoria,
                Asignee = dto.Asignee,
                ReportedBy = dto.ReportedBy,
                Departamento = dto.Departamento,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                Prioridad = dto.Prioridad,

                Avances = avances,
                Attachments = attachments,
                Historiales = historiales
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


            var idTiquete = await CrearTiqueteAsync(
                dto.Asunto,
                dto.Descripcion,
                dto.IdCategoria,
                dto.IdSubCategoria,
                currentUserId,
                dto.IdAsignee
            );

            //Una vez que todo está listo, entonces se puede hacer un log en el historial
            await _tiqueteHistorialService.RegistrarTiqueteCreadoAsync(
                    idTiquete,
                    currentUserId
                );

            //Primero se crea el tiquete para asegurar que el ID de este tiquete exista
            //Luego de eso, es posible agregar los archivos adjuntos a la base de datos
            if (dto.ArchivoAdjunto != null && dto.ArchivoAdjunto.Any())
            {
                await GuardarAdjuntosAsync(dto.ArchivoAdjunto, idTiquete, currentUserId);
            }

            return idTiquete;
        }

        //--------------------Actualizar tiquetes para el administrador-----------------------------------

        public async Task ActualizarTiqueteAsync(EditarTiqueteDto dto, string currentUserId)
        {
            //Validación 1: El usuario debe estar autenticado para actualizar un tiquete
            //Puede fallar si se prueba con http y no https por los cookies
            ValidarUsuarioActual(currentUserId);

            var tiqueteActual = await _tiqueteRepository.ObtenerEntidadPorIdAsync(dto.IdTiquete);

            //Validación 2: El tiquete debe existir para ser actualizado
            //Si el tiquete no existe, se lanza una excepción
            if (tiqueteActual == null)
            {
                throw new KeyNotFoundException("Tiquete no existe.");
            }

            //Validación 3: Si el estatus del tiquete es cerrado, entonces no se puede actualizar el tiquete
            if (tiqueteActual.IdEstatus == (int)TiqueteEstatus.Cancelado)
            {
                throw new InvalidOperationException("No se puede actualizar un tiquete cancelado.");
            }

            //Validación 4: Si el estatus del tiquete se mueve a cerrado, entonces se debe validar que el campo de resolución no esté vacío
            if (dto.IdEstatus == (int)TiqueteEstatus.Cancelado && string.IsNullOrWhiteSpace(dto.Resolucion))
            {
                throw new ArgumentException("La resolución es requerida para cerrar un tiquete.");
            }

            //Validación 5: Si el estatus del tiquete está resuelto, sólo se puede mover a en progreso
            if(tiqueteActual.IdEstatus == (int)TiqueteEstatus.Resuelto && dto.IdEstatus != (int)TiqueteEstatus.EnProceso)
            {
                throw new InvalidOperationException("El estatus del tiquete sólo se puede pasar a 'En Proceso' una vez que este ha sido marcado como 'Resuelto'");
            }

            //Validación 6: Si el estatus del tiquete está en proceso, entonces no se puede regresar a creado
            if (tiqueteActual.IdEstatus != (int)TiqueteEstatus.Creado && dto.IdEstatus == (int)TiqueteEstatus.Creado)
            {
                throw new InvalidOperationException("El estatus del tiquete no se puede pasar de otro estado a 'Creado'.");
            }
            //Antes de actualizar, se guarda el historial 
            //Capturar valores previos
            var categoriaAnterior = tiqueteActual.IdCategoria;
            var estatusAnterior = tiqueteActual.IdEstatus;


            //Si el tiquete existe, se actualizan los campos
            tiqueteActual.IdCategoria = dto.IdCategoria;
            tiqueteActual.IdSubCategoria = dto.IdSubCategoria;
            tiqueteActual.IdEstatus = dto.IdEstatus;
            tiqueteActual.Resolucion = dto.Resolucion?.Trim(); //Puede no tener nada
            tiqueteActual.UpdatedAt = DateTime.Now;
            tiqueteActual.UpdatedBy = currentUserId; //En el controller se debe pasar el id del usuario autenticado

            //Persistencia de datos -> Guardar cambios
            await _tiqueteRepository.ActualizarTiqueteAsync(tiqueteActual);

            //Si algo cambió, entonces registrar en el historial
            var categoriaAnteriorNombre = await _categoriaRepository.GetNombreAsync(categoriaAnterior);
            var categoriaNuevaNombre = await _categoriaRepository.GetNombreAsync(dto.IdCategoria);
            if (categoriaAnterior != dto.IdCategoria)
            {
                await _tiqueteHistorialService.RegistrarCambioCategoriaAsync(
                        dto.IdTiquete,
                        categoriaAnteriorNombre,
                        categoriaNuevaNombre,
                        currentUserId
                    );
            }
            var nombreEstatusAnterior = ((TiqueteEstatus)estatusAnterior).ToString();
            var nombreEstatusNuevo = ((TiqueteEstatus)dto.IdEstatus).ToString();

            if (estatusAnterior != dto.IdEstatus)
            {
                await _tiqueteHistorialService.RegistrarCambioEstadoAsync(
                    dto.IdTiquete,
                    nombreEstatusAnterior,
                    nombreEstatusNuevo,
                    currentUserId
                );
            }

            if (!string.IsNullOrWhiteSpace(dto.Resolucion) &&
                estatusAnterior != (int)TiqueteEstatus.Cancelado &&
                dto.IdEstatus == (int)TiqueteEstatus.Cancelado)
            {
                await _tiqueteHistorialService.RegistrarAvanceAsync(
                    dto.IdTiquete,
                    $"Tiquete cancelado con resolución: {dto.Resolucion}",
                    currentUserId);
            }

        }

        //--------------------------ASIGNAR DE MANERA MASIVA-----------------------
        public async Task AsignarTiquetesAsync(AsignarTiqueteDto dto, string currentUserId, bool esAdministrador)
        {
            ValidarUsuarioActual(currentUserId);

            if(!esAdministrador)
            {
                throw new UnauthorizedAccessException("Solo administradores pueden asignar tiquetes.");
            }
            
            //Obtener todos los tiquetes por la lista de IDs
            var tiquetes = await _tiqueteRepository.ObtenerTiquetesPorIdsAsync(dto.IdsTiquetes);

            //Si no hay ninguno
            if (!tiquetes.Any())
            {
                throw new KeyNotFoundException("No se encontraron tiquetes.");
            }

            //Si todo bien, iterar por cada tiquete en la lista y asignar individualmente
            foreach(var tiquete in tiquetes)
            {
                //Validación de no tocar un tiquete cancelado o resuelto
                if(tiquete.IdEstatus == (int)TiqueteEstatus.Cancelado || tiquete.IdEstatus == (int)TiqueteEstatus.Resuelto)
                {
                    continue;
                }

                tiquete.IdAsignee = dto.IdAsignee;
                tiquete.UpdatedAt = DateTime.Now;
                tiquete.UpdatedBy = currentUserId;

                //Historial de tiquetes
                await _tiqueteHistorialService.RegistrarAsignacionAsync(
                    tiquete.IdTiquete,
                    tiquete.IdAsignee,
                    tiquete.UpdatedBy);
            }

            await _tiqueteRepository.ActualizarAsignacionAsync(tiquetes);
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
            int idSubCategoria,
            string reportedBy,
            string? idAsignee
        )
        {
            var tiquete = new Tiquete
            {
                Asunto = asunto.Trim(),
                Descripcion = descripcion.Trim(),
                IdCategoria = idCategoria,
                IdSubCategoria = idSubCategoria,
                IdReportedBy = reportedBy,
                IdAsignee = idAsignee,
                IdEstatus = (int)TiqueteEstatus.Creado,
                CreatedAt = DateTime.Now
            };

            var creado = await _tiqueteRepository.AgregarTiqueteAsync(tiquete);
            return creado.IdTiquete;
        }

        private async Task GuardarAdjuntosAsync(
            List<IFormFile> archivos,
            int idTiquete,
            string currentUserId)
        {
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".pdf"};
            var attachments = new List<Attachment>();

            foreach (var archivo in archivos)
            {
                if (archivo.Length <= 0)
                    continue;

                //Validar size
                if (archivo.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("Archivo supera 5MB.");

                //Validar extensión
                var extension = Path.GetExtension(archivo.FileName).ToLower();
                if (!extensionesPermitidas.Contains(extension))
                {
                    throw new ArgumentException("Tipo de archivo no permitido.");
                }

                using var memoryStream = new MemoryStream();
                await archivo.CopyToAsync(memoryStream);

                attachments.Add(new Attachment
                {
                    IdTiquete = idTiquete,
                    File = memoryStream.ToArray(),
                    FileName = archivo.FileName,
                    UploadedBy = currentUserId,
                    UploadedAt = DateTime.Now,
                    FileSize = archivo.Length
                });
            }

            if (attachments.Any())
                await _attachmentRepository.AddRangeAsync(attachments);
        }
    }
}