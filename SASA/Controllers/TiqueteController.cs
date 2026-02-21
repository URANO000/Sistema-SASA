using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.DTOs.Tiquete.Filtros;
using DataAccess.Modelos.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.Filters;
using SASA.ViewModels.Tiquete;
using SASA.ViewModels.Tiquete.Extras;
using SASA.ViewModels.Tiquete.Filtro;
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

        public TiqueteController(ITiqueteService tiqueteService, IUsuarioService usuarioService,
            ICategoriaService categoriaService, IPrioridadService prioridadService)
        {
            _tiqueteService = tiqueteService;
            _usuarioService = usuarioService;
            _categoriaService = categoriaService;
            _prioridadService = prioridadService;
        }
        //GET: TiqueteController
        [HttpGet]
        public async Task<IActionResult> Index(TiqueteFiltroViewModel filtro)
        {
            //Primero, mapear viewmodel a dto para BLL
            var filtroDto = new TiqueteFiltroDto
            {
                Search = filtro.Search,
                Estatus = filtro.Estatus,
                Fecha = filtro.Fecha,
                PageNumber = filtro.PageNumber <= 0 ? 1 : filtro.PageNumber,
                PageSize = filtro.PageSize <= 0 ? 10 : filtro.PageSize
            };

            //Llamar a BLL con el DTO
            var result = await _tiqueteService.ObtenerTiquetesAsync(filtroDto);

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
                    Asignee = u.Asignee,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToList(),

                Filtro = new TiqueteFiltroViewModel
                {
                    Search = filtro.Search,
                    Estatus = filtro.Estatus,
                    Fecha = filtro.Fecha,
                    PageNumber = filtro.PageNumber,
                    PageSize = filtro.PageSize,
                    TotalPages = result.TotalPages
                }
            };

            //Cargo los valores para los dropdowns
            await CargarFiltrosAsync(viewModel.Filtro);

            //Una vez que todo está listo, retornamos vm
            return View(viewModel);
            
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new CrearTiqueteViewModel
            {
                Asunto = string.Empty,
                Descripcion = string.Empty,
                Categoria = 0
            };

            await CargarDropdownsAsync(model);


            return PartialView("_AddModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                var dto = new CrearTiqueteAdminDto
                {
                    Asunto = model.Asunto,
                    Descripcion = model.Descripcion,
                    IdCategoria = model.Categoria,
                    IdAsignee = model.IdAsignee
                };

                var idTiquete = await _tiqueteService.AgregarTiqueteAsync(dto, currentUserId);

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
                Asunto = tiquete.Asunto,
                Descripcion = tiquete.Descripcion,
                IdCategoria = tiquete.IdCategoria,
                IdEstatus = tiquete.IdEstatus,
                IdAsignee = tiquete.IdAsignee,
                Resolucion = tiquete.Resolucion,

            };

            await CargarDropdownsAsync(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TiqueteEditarViewModel model)
        {
            if(!ModelState.IsValid)
            {
                await CargarDropdownsAsync(model);
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
                    Asunto = model.Asunto,
                    Descripcion = model.Descripcion,
                    IdCategoria = model.IdCategoria,
                    IdEstatus = model.IdEstatus,
                    IdAsignee = model.IdAsignee,
                    Resolucion = model.Resolucion,
                    UpdatedBy = currentUserId
                };
                await _tiqueteService.ActualizarTiqueteAsync(dto);
                return Json(new
                {
                    success = true
                });

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(InvalidOperationException ex)
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
            if(tiquete == null)
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
                Asignee = tiquete.Asignee,

                CreatedAt = tiquete.CreatedAt,
                UpdatedAt = tiquete.UpdatedAt,
                
            };
            return View(model);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        //----------------------------Helpers---------------------------------------
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