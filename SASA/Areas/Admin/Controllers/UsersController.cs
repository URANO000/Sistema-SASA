using DataAccess.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SASA.ViewModels.Admin;


namespace SASA.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Admin/Users
        public IActionResult Index()
        {
            var users = _userManager.Users
                .Select(u => new UserListItemVm
                {
                    Id = u.Id,
                    Email = u.Email!,
                    Estado = u.Estado,
                    EmailConfirmed = u.EmailConfirmed,
                    AccessFailedCount = u.AccessFailedCount,
                    LockoutEnd = u.LockoutEnd
                })
                .ToList();

            return View(users);
        }

        // GET: /Admin/Users/Create
        [HttpGet]
        public IActionResult Create() => View(new CreateUserVm());

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Admin crea el usuario, pero NO puede iniciar sesión hasta activar email (RequireConfirmedEmail=true)
            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                Estado = true,
                EmailConfirmed = false,
                LockoutEnabled = true,
                PrimerNombre = vm.PrimerNombre,
                PrimerApellido = vm.PrimerApellido
            };

            // Opción simple para sprint: password temporal (luego lo mejoramos con “set password link”)
            var tempPassword = vm.TempPassword;

            var result = await _userManager.CreateAsync(user, tempPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "No se pudo crear el usuario.");
                return View(vm);
            }

            // Generar link de activación (por ahora lo logueamos)
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action("ActivateAccount", "Account", new { token = EncodeTokenPayload(user.Id, token) }, Request.Scheme);

            _logger.LogInformation("ACTIVATION LINK for {Email}: {Link}", user.Email, link);

            TempData["Success"] = "Usuario creado. Se generó un enlace de activación (revisar logs).";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Users/ToggleStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            user.Estado = !user.Estado;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Usuario {(user.Estado ? "activado" : "desactivado")}.";
            return RedirectToAction(nameof(Index));
        }

        private static string EncodeTokenPayload(string userId, string identityToken)
        {
            var raw = $"{userId}|{identityToken}";
            return Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(raw));
        }
    }


}
