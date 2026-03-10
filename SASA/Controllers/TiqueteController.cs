using BusinessLogic.Servicios.Attachments;
using BusinessLogic.Servicios.Avances;
using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.SubCategorias;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Avances;
using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.Filters;
using SASA.ViewModels.Attachments;
using SASA.ViewModels.Avances;
using SASA.ViewModels.Tiquete;
using SASA.ViewModels.Tiquete.Extras;
using SASA.ViewModels.Tiquete.Filtro;
using SASA.ViewModels.TiqueteHistoriales;
using System.Security.Claims;

namespace SASA.Controllers
{
    [RequireAuth]
    public class TiqueteController : Controller
    {
        private readonly ITiqueteService _tiqueteService;
        //Para dropdowns y autocomplete
        private readonly IUsuarioService _usuarioService;
        private readonly ICategoriaService _categoriaService;
        private readonly IPrioridadService _prioridadService;
        private readonly IAvanceService _avanceService;
        private readonly IAttachmentService _attachmentService;
        private readonly ISubCategoriaService _subCategoriasService;

        public TiqueteController(ITiqueteService tiqueteService, IUsuarioService usuarioService,
            ICategoriaService categoriaService, IPrioridadService prioridadService, IAvanceService avanceService, IAttachmentService attachmentService
            , ISubCategoriaService subCategoriaService)
        {
            _tiqueteService = tiqueteService;
            _usuarioService = usuarioService;
            _categoriaService = categoriaService;
            _prioridadService = prioridadService;
            _avanceService = avanceService;
            _attachmentService = attachmentService;
            _subCategoriasService = subCategoriaService;
        }
        //GET: TiqueteController
        [Authorize(Roles = "Administrador, Empleado Normal")]
        [HttpGet]
        public async Task<IActionResult> Index(TiqueteFiltroViewModel filtro)
        {
            //Primero, mapear viewmodel a dto para BLL
            var filtroDto = new TiqueteFiltroDto
            {
                Search = filtro.Search,
                Estatus = filtro.Estatus,
                Fecha = filtro.Fecha,
                FechaInicio = filtro.FechaInicio,
                FechaFinal = filtro.FechaFinal,
                PageNumber = filtro.PageNumber <= 0 ? 1 : filtro.PageNumber,
                PageSize = filtro.PageSize <= 0 ? 10 : filtro.PageSize
            };

            //Condicional, depende de qué rol, van a ver una lista diferente de tiquetes
            var userId = User.IsInRole("Administrador")
                ? null
                : User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _tiqueteService.ObtenerTiquetesAsync(filtroDto, userId);

            //Mapeo de resultado a ViewModel para tabla (con filtros)
            var viewModel = new TiqueteIndexViewModel
            {
                Tiquetes = result.Items.Select(u => new TiqueteListaViewModel
                {
                    IdTiquete = u.IdTiquete,
                    Asunto = u.Asunto,
                    Descripcion = u.Descripcion,
                    Resolucion = u.Resolucion,
                    Estatus = u.Estatus,
                    Categoria = u.Categoria,
                    ReportedBy = u.ReportedBy,
                    Departamento = u.Departamento,
                    Asignee = u.Asignee,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToList(),

                Filtro = new TiqueteFiltroViewModel
                {
                    Search = filtro.Search,
                    Estatus = filtro.Estatus,
                    Fecha = filtro.Fecha,
                    FechaInicio = filtro.FechaInicio,
                    FechaFinal = filtro.FechaFinal,
                    PageNumber = filtro.PageNumber,
                    PageSize = filtro.PageSize,
                    TotalPages = result.TotalPages
                },
            };

            //Cargo los valores para los dropdowns
            await CargarFiltrosAsync(viewModel.Filtro);

            //Cargo los dropdowns de el add modal
            viewModel.CrearTiquete = new CrearTiqueteViewModel
            {
                Asunto = string.Empty,
                Descripcion = string.Empty,
                Categoria = 0,
                IdSubCategoria = 0
            };

            await CargarDropdownsAsync(viewModel.CrearTiquete);

            //Una vez que todo está listo, retornamos vm
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Empleado Normal")]
        public async Task<IActionResult> Add(CrearTiqueteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns
                await CargarDropdownsAsync(model);

                return BadRequest();
            }
            try
            {
                //Guardar el usuario actual
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //Mapear viewmodel a dto
                var dto = new CrearTiqueteDto
                {
                    Asunto = model.Asunto,
                    Descripcion = model.Descripcion,
                    IdCategoria = model.Categoria,
                    IdSubCategoria = model.IdSubCategoria,
                    IdAsignee = model.IdAsignee,

                    ArchivoAdjunto = model.ArchivosAdjuntos
                };

                //Una vez que está mapeado entonces verificar si el usuario es administrador
                var esAdmin = User.IsInRole("Administrador"); //Retorna true o false

                var idTiquete = await _tiqueteService.AgregarTiqueteAsync(dto, currentUserId, esAdmin);

                return Ok(new
                {
                    success = true,
                    idTiquete
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tiquete = await _tiqueteService.ObtenerTiquetePorIdAsync(id);

            if (tiquete == null)
            {
                return NotFound();
            }

            //Mapear a viewmodel
            var model = new TiqueteEditarViewModel
            {
                IdTiquete = tiquete.IdTiquete,
                //Asunto = tiquete.Asunto,
                //Descripcion = tiquete.Descripcion,
                IdCategoria = tiquete.IdCategoria,
                IdEstatus = tiquete.IdEstatus,
                Resolucion = tiquete.Resolucion,

            };

            await CargarDropdownsAsync(model);

            return View(model);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TiqueteEditarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarDropdownsAsync(model);
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"KEY: {error.Key}");
                    foreach (var e in error.Value.Errors)
                        Console.WriteLine($"ERROR: {e.ErrorMessage}");
                }
                return Json(new
                {
                    success = false,
                    errors = ModelState
                                 .Where(x => x.Value!.Errors.Any())
                                 .ToDictionary(
                                     k => k.Key,
                                     v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                                 )
                });
            }

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var dto = new EditarTiqueteDto
                {
                    IdTiquete = model.IdTiquete,
                    IdCategoria = model.IdCategoria,
                    IdEstatus = model.IdEstatus,
                    Resolucion = model.Resolucion
                };
                await _tiqueteService.ActualizarTiqueteAsync(dto, currentUserId);
                return Json(new
                {
                    success = true
                });

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        _form = new[] { ex.Message }
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        _form = new[] { ex.Message }
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        _form = new[] { ex.Message }
                    }
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            //Obtener el tiquete
            var tiquete = await _tiqueteService.ObtenerTiquetePorIdReadAsync(id);

            //Si el servicio no retorna nada, entonces retornar not found
            if (tiquete == null)
            {
                return NotFound();
            }

            //Si el servicio retorna un tiquete, entonces mapear a viewModel
            var model = new TiqueteDetalleViewModel
            {
                IdTiquete = id,
                Asunto = tiquete.Asunto,
                Descripcion = tiquete.Descripcion,
                Resolucion = tiquete.Resolucion,
                Estatus = tiquete.Estatus,
                Categoria = tiquete.Categoria,
                ReportedBy = tiquete.ReportedBy,
                Departamento = tiquete.Departamento,
                Asignee = tiquete.Asignee,

                CreatedAt = tiquete.CreatedAt,
                UpdatedAt = tiquete.UpdatedAt,

                Avances = tiquete.Avances
                    .Select(a => new AvanceDetalleViewModel
                    {
                        AutorCorreo = a.Autor,
                        TextoAvance = a.TextoAvance,
                        CreatedAt = a.CreatedAt
                    })
                    .ToList(),
                Attachments = tiquete.Attachments
                    .Select(at => new AttachmentDetalleViewModel
                    {
                        IdAttachment = at.IdAttachment,
                        FileName = at.FileName,
                        FileSize = at.FileSize

                    })
                    .ToList(),
                Historiales = tiquete.Historiales
                    .Select(h => new TiqueteHistorialDetalleViewModel
                    {
                        PerformedAt = h.PerformedAt,
                        PerformedBy = h.PerformedBy,
                        DescripcionEvento = h.DescripcionEvento
                    })
                    .ToList()

            };
            return View(model);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAvance(int id,
                [Bind(Prefix = "NuevoAvance")] AvanceCrearViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState
                                .Where(x => x.Value!.Errors.Any())
                                .ToDictionary(
                                    k => k.Key,
                                    v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                                )
                });
            }

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var dto = new CrearAvanceDto
                {
                    TextoAvance = model.TextoAvance
                };

                await _avanceService.AgregarAvanceAsync(dto, currentUserId, id);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errors = new
                    {
                        _form = new[] { ex.Message }
                    }
                });
            }
        }


        [HttpGet]
        public async Task<IActionResult> DescargaArchivo(int id)
        {
            try
            {
                var attachment = await _attachmentService.DownloadAttachmentAsync(id);

                return File(
                    attachment.File,
                    "application/octet-stream",
                    attachment.FileName
                );
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        public IActionResult Dashboard()
        {
            return View();
        }


        //----------------------------Helpers---------------------------------------

        //Para las subcategorias
        [HttpGet]
        public async Task<IActionResult> ObtenerSubCategorias(int idCategoria)
        {
            var subcategorias = await _subCategoriasService.ObtenerSubCategoriasPorCategoria(idCategoria);
            return Json(subcategorias);
        }
        private async Task CargarDropdownsAsync(TiqueteFormViewModel model)
        {
            //Obtener datos por medio de servicios
            var categorias = await _categoriaService.ObtenerCategoriasAsync();
            var usuarios = await _usuarioService.ObtenerUsuariosTIAsync();
            var estatuses = Enum.GetValues(typeof(TiqueteEstatus))
                    .Cast<TiqueteEstatus>();


            //Mapear a selectlistitems

            model.Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.NombreCategoria
            });


            model.Asignees = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            });

            model.Estatuses = estatuses.Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = e.ToString()
            });

        }

        private async Task CargarFiltrosAsync(TiqueteFiltroViewModel model)
        {
            var estatuses = Enum.GetValues(typeof(TiqueteEstatus))
                                .Cast<TiqueteEstatus>();

            model.Estatuses = estatuses.Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.ToString(),
                Selected = e.ToString() == model.Estatus
            });
        }

    }
}