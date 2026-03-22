using BusinessLogic.Servicios.Avances;
using BusinessLogic.Servicios.Helpers;
using BusinessLogic.Servicios.TiqueteHistoriales;
using DataAccess;
using DataAccess.Identity;
using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Colas;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.ModTiquete;
using DataAccess.Modelos.Enums;
using DataAccess.Repositorios.Attachments;
using DataAccess.Repositorios.Categorias;
using DataAccess.Repositorios.Tiquetes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IHelper _helper;

        public TiqueteService(ITiqueteRepository tiqueteRepository, ICategoriaRepository categoriaRepository,
            IAvanceService avanceService, IAttachmentRepository attachmentRepository,
            ITiqueteHistorialService tiqueteHistorialService, UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            IHelper helper)
        {
            _tiqueteRepository = tiqueteRepository;
            _categoriaRepository = categoriaRepository;
            _avanceService = avanceService;
            _attachmentRepository = attachmentRepository;
            _tiqueteHistorialService = tiqueteHistorialService;
            _userManager = userManager;
            _context = context;
            _helper = helper;
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
                    Assignee = tiquete.Assignee,
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
                Assignee = dto.Assignee,
                ReportedBy = dto.ReportedBy,
                Departamento = dto.Departamento,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                Prioridad = dto.Prioridad,
                DuracionMinutos = dto.DuracionMinutos,

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
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _helper.ValidarUsuarioEstado(currentUserId);
                _helper.ValidarUsuarioActual(currentUserId);
                await ValidarCategoriaAsync(dto.IdCategoria);

                //Regla de autorización. Si es administrador entonces puede asignar tiquetes, si es empleado normal, no. 
                if (!esAdministrador && dto.IdAsignee != null)
                    throw new UnauthorizedAccessException(
                        "Un usuario normal no puede asignar tiquetes."
                    );


                var tiquete = await CrearTiqueteAsync(
                    dto.Asunto,
                    dto.Descripcion,
                    dto.IdCategoria,
                    dto.IdSubCategoria,
                    currentUserId,
                    dto.IdAsignee
                );


                //Para que exista FK a historial tiquetes (idTiquete)
                await _context.SaveChangesAsync();

                var idTiquete = tiquete.IdTiquete;

                //Una vez que todo está listo, entonces se puede hacer un log en el historial
                await _tiqueteHistorialService.RegistrarTiqueteCreadoAsync(
                        idTiquete,
                        currentUserId,
                        autoSave: false
                    );

                //Primero se crea el tiquete para asegurar que el ID de este tiquete exista
                //Luego de eso, es posible agregar los archivos adjuntos a la base de datos
                if (dto.ArchivoAdjunto != null && dto.ArchivoAdjunto.Any())
                {
                    await GuardarAdjuntosAsync(dto.ArchivoAdjunto, idTiquete, currentUserId);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                return idTiquete;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //--------------------Actualizar tiquetes para el administrador-----------------------------------

        public async Task ActualizarTiqueteAsync(EditarTiqueteDto dto, string currentUserId)
        {
            //Validación 0:Validar que es usuario activo
            await _helper.ValidarUsuarioEstado(currentUserId);

            //Validación 1: El usuario debe estar autenticado para actualizar un tiquete
            //Puede fallar si se prueba con http y no https por los cookies
            _helper.ValidarUsuarioActual(currentUserId);

            var tiqueteActual = await _tiqueteRepository.ObtenerEntidadPorIdAsync(dto.IdTiquete);

            //Validación 2: El tiquete debe existir para ser actualizado
            if (tiqueteActual == null)
            {
                throw new KeyNotFoundException("Tiquete no existe.");
            }


            //Validación 3: No se realizaron cambios
            bool sinCambios =
                   tiqueteActual.IdCategoria == dto.IdCategoria &&
                   tiqueteActual.IdSubCategoria == dto.IdSubCategoria &&
                   tiqueteActual.IdEstatus == dto.IdEstatus &&
                   (tiqueteActual.Resolucion ?? "").Trim() == (dto.Resolucion ?? "").Trim();

            if (sinCambios)
            {
                throw new InvalidOperationException("No se realizaron cambios en el tiquete.");
            }

            //Validación 4: Si el estatus del tiquete es cerrado, entonces no se puede actualizar el tiquete
            if (tiqueteActual.IdEstatus == (int)TiqueteEstatus.Cancelado)
            {
                throw new InvalidOperationException("No se puede actualizar un tiquete cancelado.");
            }

            //Validación 5: Si el estatus del tiquete se mueve a cerrado, entonces se debe validar que el campo de resolución no esté vacío
            if (dto.IdEstatus == (int)TiqueteEstatus.Cancelado && string.IsNullOrWhiteSpace(dto.Resolucion))
            {
                throw new ArgumentException("La resolución es requerida para cerrar un tiquete.");
            }

            //Validación 6: Si el estatus del tiquete está resuelto, sólo se puede mover a en progreso
            if(tiqueteActual.IdEstatus == (int)TiqueteEstatus.Resuelto && dto.IdEstatus != (int)TiqueteEstatus.EnProceso)
            {
                throw new InvalidOperationException("El estatus del tiquete sólo se puede pasar a 'En Proceso' una vez que este ha sido marcado como 'Resuelto'");
            }

            //Validación 7: Si el estatus del tiquete está en proceso, entonces no se puede regresar a creado
            if (tiqueteActual.IdEstatus != (int)TiqueteEstatus.Creado && dto.IdEstatus == (int)TiqueteEstatus.Creado)
            {
                throw new InvalidOperationException("El estatus del tiquete no se puede pasar de otro estado a 'Creado'.");
            }

            //Antes de actualizar, se guarda el historial 
            var categoriaAnterior = tiqueteActual.IdCategoria;
            var estatusAnterior = tiqueteActual.IdEstatus;

            //Validación 8: Si el tiquete cambia de resuelto a en proceso, reinsertar a cola
            bool entraACola =
                (estatusAnterior == (int)TiqueteEstatus.Resuelto)
                &&
                dto.IdEstatus == (int)TiqueteEstatus.EnProceso;

            if (entraACola && tiqueteActual.IdAsignee != null)
            {
                var siguienteOrden = await _tiqueteRepository
                    .ObtenerSiguienteOrdenColaAsync(tiqueteActual.IdAsignee);

                tiqueteActual.OrdenCola = siguienteOrden;
            }

            //Si el tiquete existe, se actualizan los campos
            tiqueteActual.IdCategoria = dto.IdCategoria;
            tiqueteActual.IdSubCategoria = dto.IdSubCategoria;
            tiqueteActual.IdEstatus = dto.IdEstatus;
            tiqueteActual.Resolucion = dto.Resolucion?.Trim();
            tiqueteActual.UpdatedAt = DateTime.UtcNow;
            tiqueteActual.UpdatedBy = currentUserId;

            //Validación 9: Tiquete sale de cola si el estado cambia a cancelado o resuelto
            bool saleDeCola =
                dto.IdEstatus == (int)TiqueteEstatus.Cancelado ||
                dto.IdEstatus == (int)TiqueteEstatus.Resuelto;

            if (saleDeCola && tiqueteActual.OrdenCola != null)
            {
                tiqueteActual.OrdenCola = null;
            }

            //Guardar cambios
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
            await _helper.ValidarUsuarioEstado(currentUserId);
            _helper.ValidarUsuarioActual(currentUserId);

            if(!esAdministrador)
            {
                throw new UnauthorizedAccessException("Solo administradores pueden asignar tiquetes.");
            }

            //---------------------------------INICIA LA TRANSACCIÓN--------------------------------------------
            //--------------------------------------------------------------------------------------------------

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //Obtener todos los tiquetes por la lista de IDs
                var tiquetes = await _tiqueteRepository.ObtenerTiquetesPorIdsAsync(dto.IdsTiquetes);

                //Si no hay ninguno
                if (!tiquetes.Any())
                {
                    throw new KeyNotFoundException("No se encontraron tiquetes.");
                }

                var user = await _userManager.FindByIdAsync(dto.IdAssignee);
                _helper.ValidarUsuarioExiste(user);
                

                //Para historial de tiquetes, abajo
                var nombreCompleto = user.PrimerNombre + " " + user.PrimerApellido;

                //Para colas
                decimal siguienteOrden = 0;

                if (dto.IdAssignee != null)
                {
                    siguienteOrden =
                        await _tiqueteRepository.ObtenerSiguienteOrdenColaAsync(dto.IdAssignee);
                }

                //Si todo bien, iterar por cada tiquete en la lista y asignar individualmente
                foreach (var tiquete in tiquetes)
                {
                    //Validación de no tocar un tiquete cancelado o resuelto
                    if (tiquete.IdEstatus == (int)TiqueteEstatus.Cancelado || tiquete.IdEstatus == (int)TiqueteEstatus.Resuelto)
                    {
                        continue;
                    }

                    //Para historial y colas
                    var asignadoAnterior = tiquete.IdAsignee;
                    var usuarioAnterior = await _userManager.FindByIdAsync(asignadoAnterior);
                    _helper.ValidarUsuarioExiste(usuarioAnterior);

                    var nombreAnterior = usuarioAnterior.PrimerNombre + " " + usuarioAnterior.PrimerApellido;

                    //Para colas nada más
                    bool cambiaAssignee = asignadoAnterior != dto.IdAssignee;

                    if (cambiaAssignee)
                    {
                        if(dto.IdAssignee == null)
                        {
                            tiquete.OrdenCola = null;
                        }
                        else
                        {
                            tiquete.OrdenCola = siguienteOrden;
                            siguienteOrden += 1000m;
                        }
                    }

                    //De la operación para reasignar
                    tiquete.IdAsignee = dto.IdAssignee;
                    tiquete.UpdatedAt = DateTime.UtcNow;
                    tiquete.UpdatedBy = currentUserId;

                    //Historial de tiquetes

                    await _tiqueteHistorialService.RegistrarAsignacionAsync(
                        tiquete.IdTiquete,
                        nombreAnterior ?? "Sin asignado anterior",
                        nombreCompleto ?? "Sin nombre asignado",
                        tiquete.UpdatedBy,
                        autoSave: false);
                }

                await _tiqueteRepository.ActualizarAsignacionAsync(tiquetes);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                //Si algo falla, abortar la transacción
                await transaction.RollbackAsync();
                throw;
            }
        }


        //---------------------------Zona de colas ---------------------------------------------
        public async Task<List<ColaTiqueteDto>> GetColaPersonalAsync(string currentUserId)
        {
            //Primeramente, validar y asegurar que usuario es admin
            _helper.ValidarUsuarioActual(currentUserId);

            return await _tiqueteRepository.GetColaPersonalAsync(currentUserId);
        }

        public async Task<List<ColaPorAssigneeDto>> GetColasGlobalAsync()
        {
            return await _tiqueteRepository.GetColasGlobalAsync();
        }

        public async Task<decimal> ReordenarAsync(int idTiquete, decimal? ordenAnterior, decimal? ordenSiguiente)
        {
            decimal nuevoOrden;

            if (ordenAnterior == null && ordenSiguiente == null)
                throw new Exception("Lista vacía");

            if (ordenAnterior == null)
            {
                //Mover arriba
                nuevoOrden = ordenSiguiente.Value / 2m;
            }
            else if (ordenSiguiente == null)
            {
                //Mover abajo
                nuevoOrden = ordenAnterior.Value + 1000m;
            }
            else
            {
                // Between two items
                nuevoOrden = _tiqueteRepository.CalcularOrdenEntre(ordenAnterior.Value, ordenSiguiente.Value);
            }

            var tiquete = await _context.Tiquetes.FindAsync(idTiquete);
            tiquete.OrdenCola = nuevoOrden;

            await _context.SaveChangesAsync();

            return nuevoOrden;
        }

        //---------------------------Dashboard--------------------------------------------------
        public async Task<int> ContarTiquetesAsync()
        {
            return await _tiqueteRepository.ContarTiquetesAsync();
        }

        public async Task<List<TiquetesPorEstadoDto>> ObtenerTiquetesPorEstadoAsync()
        {
            return await _tiqueteRepository.ObtenerTiquetesPorEstadoAsync();
        }

        public async Task<List<TiquetesPorDiaDto>> ObtenerTiquetesUltimos7DiasAsync()
        {
            return await _tiqueteRepository.ObtenerTiquetesUltimos7DiasAsync();
        }

        public async Task<double> PromedioResolucionAsync()
        {
            return await _tiqueteRepository.PromedioResolucionAsync();
        }

        public async Task<List<TiquetesPorEstadoDto>> ObtenerTiquetesVencidosPorEstadoAsync()
        {
            return await _tiqueteRepository.ObtenerTiquetesVencidosPorEstadoAsync();
        }


        //----------------------------HELPERS (DRY)---------------------------------------------

        private async Task ValidarCategoriaAsync(int idCategoria)
        {
            var categoriaExiste = await _categoriaRepository.ExisteAsync(idCategoria);
            if (!categoriaExiste)
                throw new ArgumentException("Categoria no existe.");
        }

        private async Task<Tiquete> CrearTiqueteAsync(
            string asunto,
            string descripcion,
            int idCategoria,
            int idSubCategoria,
            string reportedBy,
            string? idAsignee
        )
        {

            //Un poco de lógica para colas---------------------------------
            decimal? ordenCola = null;

            //Si el tiquete es assignado, entonces debe de entrar a la cola del usuario asignado
            if (!string.IsNullOrEmpty(idAsignee))
            {
                ordenCola = await _tiqueteRepository.ObtenerSiguienteOrdenColaAsync(idAsignee);
            }

            var tiquete = new Tiquete
            {
                Asunto = asunto.Trim(),
                Descripcion = descripcion.Trim(),
                IdCategoria = idCategoria,
                IdSubCategoria = idSubCategoria,
                IdReportedBy = reportedBy,
                IdAsignee = idAsignee,
                OrdenCola = ordenCola,
                IdEstatus = (int)TiqueteEstatus.Creado,
                CreatedAt = DateTime.UtcNow
            };

            var creado = await _tiqueteRepository.AgregarTiqueteAsync(tiquete);
            return tiquete;
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
                if (archivo.Length > 2 * 1024 * 1024)
                    throw new ArgumentException("Archivo supera 2MB.");

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
                    UploadedAt = DateTime.UtcNow,
                    FileSize = archivo.Length
                });
            }

            if (attachments.Any())
                await _attachmentRepository.AddRangeAsync(attachments);
        }

    }
}