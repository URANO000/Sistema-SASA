using System.Text;
using DataAccess.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SASA.ViewModels.Auth;

namespace SASA.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Buscar por email (no revelar si existe o no)
            var user = await _userManager.FindByEmailAsync(vm.Email!);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(vm);
            }

            // Si el admin desactivó la cuenta
            if (!user.Estado)
            {
                ModelState.AddModelError(string.Empty, "Tu cuenta está desactivada. Contacta a un administrador.");
                return View(vm);
            }

            // (historia #4): lockoutOnFailure = true
            var result = await _signInManager.PasswordSignInAsync(
                user,
                vm.Password!,
                isPersistent: false,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
                return Redirect("/Home/Index");

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente por múltiples intentos fallidos. Intenta más tarde.");
                return View(vm);
            }

            if (result.IsNotAllowed)
            {
                // Con RequireConfirmedEmail = true, cae aquí si no ha confirmado el email
                ModelState.AddModelError(string.Empty, "Debes activar tu cuenta desde el enlace enviado a tu correo antes de iniciar sesión.");
                return View(vm);
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
            return View(vm);
        }

        [HttpPost("/logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost("/forgot-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Siempre responder igual para no filtrar si existe el correo
            var genericMsg = "Si el correo existe, se enviará un enlace de recuperación.";

            var user = await _userManager.FindByEmailAsync(vm.Email!);
            if (user is null)
            {
                TempData["Success"] = genericMsg;
                return RedirectToAction(nameof(Login));
            }

            // Generar token real
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var payload = EncodeTokenPayload(user.Id, resetToken);

            var callbackUrl = Url.Action(
                nameof(ResetPassword),
                "Account",
                new { token = payload },
                protocol: Request.Scheme
            );

            // Temporal (hasta tener SMTP)
            _logger.LogInformation("RESET PASSWORD LINK for {Email}: {Link}", user.Email, callbackUrl);

            TempData["Success"] = genericMsg;
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/reset-password/{token}")]
        public IActionResult ResetPassword(string token)
        {
            var decoded = DecodeTokenPayload(token);
            if (!decoded.ok)
            {
                TempData["Error"] = "Token inválido o mal formado.";
                return RedirectToAction(nameof(Login));
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost("/reset-password/{token}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordViewModel vm)
        {
            vm.Token = token;

            if (!ModelState.IsValid) return View(vm);

            if (vm.NewPassword != vm.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                return View(vm);
            }

            var decoded = DecodeTokenPayload(token);
            if (!decoded.ok)
            {
                ModelState.AddModelError(string.Empty, "Token inválido o mal formado.");
                return View(vm);
            }

            var user = await _userManager.FindByIdAsync(decoded.userId);
            if (user is null)
            {
                // No revelar
                TempData["Success"] = "Contraseña actualizada. Ahora puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, decoded.identityToken, vm.NewPassword!);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "No se pudo restablecer la contraseña. Verifica el enlace e intenta de nuevo.");
                return View(vm);
            }

            TempData["Success"] = "Contraseña actualizada. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/activate-account/{token}")]
        public async Task<IActionResult> ActivateAccount(string token)
        {
            var decoded = DecodeTokenPayload(token);
            if (!decoded.ok)
            {
                TempData["Error"] = "Token inválido o mal formado.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(decoded.userId);
            if (user is null)
            {
                TempData["Error"] = "No se encontró el usuario.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ConfirmEmailAsync(user, decoded.identityToken);

            if (!result.Succeeded)
            {
                TempData["Error"] = "No se pudo activar la cuenta. El enlace podría haber expirado.";
                return RedirectToAction(nameof(Login));
            }

            TempData["Success"] = "Cuenta activada. Ahora crea tu contraseña.";
            return RedirectToAction(nameof(SetPassword), new { token });
        }

        [HttpGet("/Account/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }


        private static string EncodeTokenPayload(string userId, string identityToken)
        {
            var raw = $"{userId}|{identityToken}";
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(raw));
        }

        private static (bool ok, string userId, string identityToken) DecodeTokenPayload(string payload)
        {
            try
            {
                var bytes = WebEncoders.Base64UrlDecode(payload);
                var raw = Encoding.UTF8.GetString(bytes);

                var parts = raw.Split('|', 2);
                if (parts.Length != 2) return (false, "", "");

                return (true, parts[0], parts[1]);
            }
            catch
            {
                return (false, "", "");
            }
        }

        [HttpGet("/set-password/{token}")]
        public async Task<IActionResult> SetPassword(string token)
        {
            var decoded = DecodeTokenPayload(token);
            if (!decoded.ok)
            {
                TempData["Error"] = "Token inválido o mal formado.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(decoded.userId);
            if (user is null)
            {
                TempData["Error"] = "No se encontró el usuario.";
                return RedirectToAction(nameof(Login));
            }

            // Solo permite si ya se confirmó el correo
            if (!user.EmailConfirmed)
            {
                TempData["Error"] = "Debes activar tu cuenta antes de crear una contraseña.";
                return RedirectToAction(nameof(Login));
            }

            // Si ya tiene password, omite ese flujo
            if (await _userManager.HasPasswordAsync(user))
            {
                TempData["Success"] = "Tu cuenta ya tiene contraseña. Puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            return View(new SetPasswordViewModel { Token = token });
        }

        [HttpPost("/set-password/{token}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(string token, SetPasswordViewModel vm)
        {
            vm.Token = token;

            if (!ModelState.IsValid)
                return View(vm);

            var decoded = DecodeTokenPayload(token);
            if (!decoded.ok)
            {
                TempData["Error"] = "Token inválido o mal formado.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByIdAsync(decoded.userId);
            if (user is null)
            {
                TempData["Error"] = "No se encontró el usuario.";
                return RedirectToAction(nameof(Login));
            }

            if (!user.EmailConfirmed)
            {
                TempData["Error"] = "Debes activar tu cuenta antes de crear una contraseña.";
                return RedirectToAction(nameof(Login));
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                TempData["Success"] = "Tu cuenta ya tiene contraseña. Puedes iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            // Se crea la contraseña por primera vez
            var result = await _userManager.AddPasswordAsync(user, vm.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                return View(vm);
            }

            TempData["Success"] = "Contraseña creada. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }


    }
}
