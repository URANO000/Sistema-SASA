using BusinessLogic.Servicios.Categorias;
using BusinessLogic.Servicios.Prioridad;
using BusinessLogic.Servicios.SubCategorias;
using DataAccess.Modelos.DTOs.Categoria;
using DataAccess.Modelos.DTOs.SubCategoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SASA.Filters;
using BusinessLogic.Servicios.Helpers;
using SASA.ViewModels.Categoria;

namespace SASA.Controllers
{
    [RequireAuth]
    [Authorize(Roles = "Administrador")]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ISubCategoriaService _subCategoriaService;
        private readonly IPrioridadService _prioridadService;
        private readonly IHelper _helper;

        public CategoriaController(
            ICategoriaService categoriaService,
            ISubCategoriaService subCategoriaService,
            IPrioridadService prioridadService,
            IHelper helper)
        {
            _categoriaService = categoriaService;
            _subCategoriaService = subCategoriaService;
            _prioridadService = prioridadService;
            _helper = helper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string tab = "categorias",
            string? categoriaBuscar = null,
            int categoriaPagina = 1,
            string? subCategoriaBuscar = null,
            int? subCategoriaFiltroIdCategoria = null,
            int? subCategoriaFiltroIdPrioridad = null,
            int subCategoriaPagina = 1)
        {
            var vm = await ConstruirViewModelAsync(
                tab,
                categoriaBuscar,
                categoriaPagina,
                subCategoriaBuscar,
                subCategoriaFiltroIdCategoria,
                subCategoriaFiltroIdPrioridad,
                subCategoriaPagina);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(CrearCategoriaViewModel model)
        {
            // If request is AJAX, return JSON responses similar to Tiquete controller
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
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

                TempData["Error"] = "Verifica los datos de la categoría.";
                var vm = await ConstruirViewModelAsync("categorias", null, 1, null, null, null, 1);
                vm.CrearCategoria = model;
                return View("Index", vm);
            }

            var result = await _categoriaService.CrearAsync(new CrearCategoriaDto
            {
                NombreCategoria = model.NombreCategoria
            });

            if (isAjax)
            {
                if (result.Ok)
                    return Json(new { success = true });

                return Json(new { success = false, errors = new { _form = new[] { result.Mensaje } } });
            }

            TempData[result.Ok ? "Success" : "Error"] = result.Mensaje;
            return RedirectToAction(nameof(Index), new { tab = "categorias" });
        }

        [HttpGet]
        public async Task<IActionResult> EditarCategoria(int id)
        {
            var dto = await _categoriaService.ObtenerParaEditarAsync(id);
            if (dto == null)
                return NotFound();

            return View(new EditarCategoriaViewModel
            {
                IdCategoria = dto.IdCategoria,
                NombreCategoria = dto.NombreCategoria
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCategoria(EditarCategoriaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _categoriaService.EditarAsync(new EditarCategoriaDto
            {
                IdCategoria = model.IdCategoria,
                NombreCategoria = model.NombreCategoria
            });

            if (!result.Ok)
            {
                ModelState.AddModelError(string.Empty, result.Mensaje);
                return View(model);
            }

            TempData["Success"] = result.Mensaje;
            return RedirectToAction(nameof(Index), new { tab = "categorias" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSubCategoria(CrearSubCategoriaViewModel model)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
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

                TempData["Error"] = "Verifica los datos de la subcategoría.";
                var vm = await ConstruirViewModelAsync("subcategorias", null, 1, null, null, null, 1);
                vm.CrearSubCategoria = model;
                vm.CrearSubCategoria.Categorias = vm.CategoriasDropdown;
                vm.CrearSubCategoria.Prioridades = vm.PrioridadesDropdown;
                return View("Index", vm);
            }

            var result = await _subCategoriaService.CrearAsync(new CrearSubCategoriaDto
            {
                IdCategoria = model.IdCategoria,
                IdPrioridad = model.IdPrioridad,
                NombreSubCategoria = model.NombreSubCategoria
            });

            if (isAjax)
            {
                if (result.Ok)
                    return Json(new { success = true });

                return Json(new { success = false, errors = new { _form = new[] { result.Mensaje } } });
            }

            TempData[result.Ok ? "Success" : "Error"] = result.Mensaje;
            return RedirectToAction(nameof(Index), new { tab = "subcategorias" });
        }

        [HttpGet]
        public async Task<IActionResult> EditarSubCategoria(int id)
        {
            var dto = await _subCategoriaService.ObtenerParaEditarAsync(id);
            if (dto == null)
                return NotFound();

            var categorias = await _categoriaService.ObtenerTodasAsync();
            var prioridades = await _prioridadService.ObtenerPrioridadesAsync();

            var vm = new EditarSubCategoriaViewModel
            {
                IdSubCategoria = dto.IdSubCategoria,
                IdCategoria = dto.IdCategoria,
                IdPrioridad = dto.IdPrioridad,
                NombreSubCategoria = dto.NombreSubCategoria,
                Categorias = categorias.Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria
                }).ToList(),
                Prioridades = prioridades.Select(p => new SelectListItem
                {
                    Value = p.IdPrioridad.ToString(),
                    Text = p.NombrePrioridad
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarSubCategoria(EditarSubCategoriaViewModel model)
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            var prioridades = await _prioridadService.ObtenerPrioridadesAsync();

            model.Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.NombreCategoria
            }).ToList();

            model.Prioridades = prioridades.Select(p => new SelectListItem
            {
                Value = p.IdPrioridad.ToString(),
                Text = p.NombrePrioridad
            }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var result = await _subCategoriaService.EditarAsync(new EditarSubCategoriaDto
            {
                IdSubCategoria = model.IdSubCategoria,
                IdCategoria = model.IdCategoria,
                IdPrioridad = model.IdPrioridad,
                NombreSubCategoria = model.NombreSubCategoria
            });

            if (!result.Ok)
            {
                ModelState.AddModelError(string.Empty, result.Mensaje);
                return View(model);
            }

            TempData["Success"] = result.Mensaje;
            return RedirectToAction(nameof(Index), new { tab = "subcategorias" });
        }

        private async Task<CategoriaGestionViewModel> ConstruirViewModelAsync(
            string tab,
            string? categoriaBuscar,
            int categoriaPagina,
            string? subCategoriaBuscar,
            int? subCategoriaFiltroIdCategoria,
            int? subCategoriaFiltroIdPrioridad,
            int subCategoriaPagina)
        {
            const int pageSize = 10;

            var categoriasPaginadas = await _categoriaService.ObtenerCategoriasAsync(new FiltroCategoriaDto
            {
                Buscar = categoriaBuscar,
                PageNumber = categoriaPagina < 1 ? 1 : categoriaPagina,
                PageSize = pageSize
            });

            var subCategoriasPaginadas = await _subCategoriaService.ObtenerSubCategoriasAsync(new FiltroSubCategoriaDto
            {
                Buscar = subCategoriaBuscar,
                IdCategoria = subCategoriaFiltroIdCategoria,
                IdPrioridad = subCategoriaFiltroIdPrioridad,
                PageNumber = subCategoriaPagina < 1 ? 1 : subCategoriaPagina,
                PageSize = pageSize
            });

            var categoriasTodas = await _categoriaService.ObtenerTodasAsync();
            var prioridades = await _prioridadService.ObtenerPrioridadesAsync();

            var categoriasDropdown = categoriasTodas
                .Select(c => new SelectListItem
                {
                    Value = c.IdCategoria.ToString(),
                    Text = c.NombreCategoria
                }).ToList();

            var prioridadesDropdown = prioridades
                .Select(p => new SelectListItem
                {
                    Value = p.IdPrioridad.ToString(),
                    Text = p.NombrePrioridad
                }).ToList();

            // Set DuracionDisplay for each subcategory item using helper
            foreach (var item in subCategoriasPaginadas.Items)
            {
                item.DuracionDisplay = _helper.FormatearDuracionDesdeMinutos(item.DuracionMinutos);
            }

            return new CategoriaGestionViewModel
            {
                TabActiva = tab,
                CategoriasPaginadas = categoriasPaginadas,
                SubCategoriasPaginadas = subCategoriasPaginadas,
                CategoriaBuscar = categoriaBuscar,
                CategoriaPagina = categoriaPagina,
                SubCategoriaBuscar = subCategoriaBuscar,
                SubCategoriaFiltroIdCategoria = subCategoriaFiltroIdCategoria,
                SubCategoriaFiltroIdPrioridad = subCategoriaFiltroIdPrioridad,
                SubCategoriaPagina = subCategoriaPagina,
                CategoriasDropdown = categoriasDropdown,
                PrioridadesDropdown = prioridadesDropdown,
                CrearCategoria = new CrearCategoriaViewModel(),
                CrearSubCategoria = new CrearSubCategoriaViewModel
                {
                    Categorias = categoriasDropdown,
                    Prioridades = prioridadesDropdown
                }
            };
        }
    }
}