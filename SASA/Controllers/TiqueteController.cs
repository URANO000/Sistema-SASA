
using BusinessLogic.Servicios.Tiquetes;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;
using SASA.ViewModels.Tiquete;

namespace SASA.Controllers
{
    [RequireAuth]
    public class TiqueteController : Controller
    {
        private readonly ITiqueteService _tiqueteService;

        public TiqueteController(ITiqueteService tiqueteService)
        {
            _tiqueteService = tiqueteService;
        }
        //GET: TiqueteController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var tiqueteDTO = await _tiqueteService.ObtenerTiquetesAsync();

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

            return View(tiquetes);
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
