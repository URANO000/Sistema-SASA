using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Tiquete;
using DataAccess.Modelos.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.Filters;
using SASA.ViewModels.Tiquete;
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
        public async Task<IActionResult> Index()
        {
            var tiqueteDTO = await _tiqueteService.ObtenerTiquetesAsync();

            var model = tiqueteDTO
                .Select(u => new TiqueteListaViewModel
                {
                    IdTiquete = u.IdTiquete,
                    Asunto = u.Asunto,
                    Descripcion = u.Descripcion,
                    Resolucion = u.Resolucion,
                    Estatus = u.Estatus,
                    Prioridad = u.Prioridad,
                    Categoria = u.Categoria,
                    Cola = u.Cola,
                    ReportedBy = u.ReportedBy,
                    Asignee = u.Asignee,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categorias = await _categoriaService.ObtenerCategoriasAsync();
            var prioridades = await _prioridadService.ObtenerPrioridadesAsync();
            var usuarios = await _usuarioService.ObtenerUsuariosTIAsync();

            var model = new CrearTiqueteViewModel
            {
                Categorias = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria
                }),

                Prioridades = prioridades.Select(p => new SelectListItem
                {
                    Value = p.IdPrioridad.ToString(),
                    Text = p.NombrePrioridad
                }),

                Asignees = usuarios.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.UserName
                }),
                Asunto = string.Empty,
                Descripcion = string.Empty,
                Categoria = 0,
                Prioridad = 0,
            };


            return PartialView("_AddModal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CrearTiqueteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns
                model.Categorias = (IEnumerable<SelectListItem>?)await _categoriaService.ObtenerCategoriasAsync();
                model.Prioridades = (IEnumerable<SelectListItem>?)await _prioridadService.ObtenerPrioridadesAsync();
                model.Asignees = (IEnumerable<SelectListItem>?)await _usuarioService.ObtenerUsuariosTIAsync();

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
                    IdPrioridad = model.Prioridad,
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
        public IActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}