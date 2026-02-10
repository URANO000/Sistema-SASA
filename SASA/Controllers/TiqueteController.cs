
using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.Tiquetes;
using BusinessLogic.Servicios.Usuarios;
using DataAccess.Modelos.DTOs.Tiquete;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.Tiquete;

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
            var categorias = await _categoriaService.ObtenerCategoriasAsync();
            var prioridades = await _prioridadService.ObtenerPrioridadesAsync();
            var usuariosTI = await _usuarioService.ObtenerUsuariosTIAsync();

            var tiquetes = tiqueteDTO.Select(u => new TiqueteListaViewModel
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
            }).ToList();

            //Crear un viewmodel para enviar las listas de categorias, prioridades y usuarios para los filtros
            var model = new TiqueteIndexViewModel
            {
                Tiquetes = tiquetes,
                CategoriasDisponibles = categorias,
                PrioridadesDisponibles = prioridades,
                UsuariosTIDisponibles = usuariosTI,

            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CrearTiqueteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //Guardar el usuario actual
                var currentUserId = User.FindFirst("Id")?.Value;

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

            }catch(Exception ex)
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
