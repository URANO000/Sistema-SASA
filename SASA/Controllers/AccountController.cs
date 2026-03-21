using BusinessLogic.Servicios.Autenticacion;
using BusinessLogic.Servicios.Correo;
using DataAccess.Identity;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using SASA.Configuration;
using SASA.ViewModels.Auth;
using System.Text;

namespace SASA.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ICorreoNotificacionesService _correoNotificaciones;
        private readonly AppSettings _appSettings;
        private readonly IAntiforgery _antiforgery;
        private readonly ILoginAttemptService _loginAttemptService;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger,
            ICorreoNotificacionesService correoNotificaciones,
            IOptions<AppSettings> appSettings,
            IAntiforgery antiforgery,
            ILoginAttemptService loginAttemptService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _correoNotificaciones = correoNotificaciones;
            _appSettings = appSettings.Value;
            _antiforgery = antiforgery;
            _loginAttemptService = loginAttemptService;
        }

        [AllowAnonymous]
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // captura datos del request una sola vez
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            // Buscar por email (no revelar si existe o no)
            var user = await _userManager.FindByEmailAsync(vm.Email!);
            if (user is null)
            {
                // intento fallido (no existe usuario)
                await _loginAttemptService.RegistrarAsync(vm.Email!, null, false, "UserNotFound", ip, userAgent);

                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(vm);
            }

            // Si el admin desactivó la cuenta
            if (!user.Estado)
            {
                // intento fallido (deshabilitado)
                await _loginAttemptService.RegistrarAsync(vm.Email!, user.Id, false, "Disabled", ip, userAgent);

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
            {
                // successful login
                await _loginAttemptService.RegistrarAsync(vm.Email!, user.Id, true, null, ip, userAgent);
                return Redirect("/Home/Index");
            }

            if (result.IsLockedOut)
            {
                // locked out
                await _loginAttemptService.RegistrarAsync(vm.Email!, user.Id, false, "LockedOut", ip, userAgent);

                ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente por múltiples intentos fallidos. Intenta más tarde.");
                return View(vm);
            }

            if (result.IsNotAllowed)
            {
                // not allowed: email not confirmed, etc.
                await _loginAttemptService.RegistrarAsync(vm.Email!, user.Id, false, "NotAllowed", ip, userAgent);

                ModelState.AddModelError(string.Empty, "Debes activar tu cuenta desde el enlace enviado a tu correo antes de iniciar sesión.");
                return View(vm);
            }

            // not successful for other reasons (e.g. password incorrect)
            await _loginAttemptService.RegistrarAsync(vm.Email!, user.Id, false, "InvalidCredentials", ip, userAgent);

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

        [AllowAnonymous]
        [HttpGet("/forgot-password")]
        public IActionResult ForgotPassword() => View();

        [AllowAnonymous]
        [HttpPost("/forgot-password")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Siempre responder igual para no filtrar si existe el correo
            var genericMsg = "Si el correo existe, se enviará un enlace de recuperación.";

            var user = await _userManager.FindByEmailAsync(vm.Email!);

            // Si no existe o está inactivo -> misma respuesta
            if (user is null || !user.Estado)
            {
                TempData["Success"] = genericMsg;
                return RedirectToAction(nameof(Login));
            }

            // Generar token real
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var payload = EncodeTokenPayload(user.Id, resetToken);

            // Construir link usando BaseUrl (como UsuarioController)
            var baseUrl = (_appSettings.BaseUrl ?? "").TrimEnd('/');
            var resetLink = $"{baseUrl}/reset-password/{payload}";

            // Nombre para el correo
            var toName = (user.UserName ?? user.Email ?? "Usuario").Trim();

            // Enviar correo con Graph (si falla, NO rompe)
            _ = await _correoNotificaciones.EnviarRecuperacionContrasenaAsync(user.Email!, toName, resetLink);

            TempData["Success"] = "Si el correo existe, se enviará un enlace de recuperación.";
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        [HttpGet("/reset-password/{token}")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            await _signInManager.SignOutAsync();

            _antiforgery.GetAndStoreTokens(HttpContext);

            var validation = await ValidateResetPasswordTokenAsync(token);
            if (!validation.ok)
            {
                TempData["Error"] = validation.errorMessage;
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("/reset-password/{token}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordViewModel model)
        {
            if (token != model.Token)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var validation = await ValidateResetPasswordTokenAsync(model.Token);
            if (!validation.ok || validation.user is null)
            {
                TempData["Error"] = validation.errorMessage;
                return RedirectToAction(nameof(ForgotPassword));
            }

            var decoded = DecodeTokenPayload(model.Token);
            if (!decoded.ok)
            {
                TempData["Error"] = "El enlace de recuperación es inválido, ya fue utilizado o expiró. Solicita uno nuevo.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var resetResult = await _userManager.ResetPasswordAsync(validation.user, decoded.identityToken, model.NewPassword!);
            if (!resetResult.Succeeded)
            {
                foreach (var e in resetResult.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                return View(model);
            }

            await _userManager.UpdateSecurityStampAsync(validation.user);

            TempData["Success"] = "Tu contraseña fue restablecida correctamente.";
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
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
            return Redirect($"/set-password/{token}");
        }

        [AllowAnonymous]
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

        private async Task<(bool ok, ApplicationUser? user, string errorMessage)> ValidateResetPasswordTokenAsync(string payload)
        {
            var decoded = DecodeTokenPayload(payload);
            if (!decoded.ok)
            {
                return (false, null, "El enlace de recuperación es inválido o mal formado.");
            }

            var user = await _userManager.FindByIdAsync(decoded.userId);
            if (user is null || !user.Estado)
            {
                return (false, null, "El enlace de recuperación es inválido o ya no está disponible.");
            }

            var isValid = await _userManager.VerifyUserTokenAsync(
                user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword",
                decoded.identityToken);

            if (!isValid)
            {
                return (false, null, "El enlace de recuperación es inválido, ya fue utilizado o expiró. Solicita uno nuevo.");
            }

            return (true, user, string.Empty);
        }


        [AllowAnonymous]
        [HttpGet("/set-password/{token}")]
        public async Task<IActionResult> SetPassword(string token)
        {
            await _signInManager.SignOutAsync();

            _antiforgery.GetAndStoreTokens(HttpContext);

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

        [AllowAnonymous]
        [HttpPost("/set-password/{token}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(string token, SetPasswordViewModel vm)
        {
            // Si viene token en la URL, úsalo; si no, usa el hidden
            vm.Token = token ?? vm.Token;

            if (!ModelState.IsValid)
                return View(vm);

            var decoded = DecodeTokenPayload(vm.Token);
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

            if (vm.NewPassword != vm.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                return View(vm);
            }

            var result = await _userManager.AddPasswordAsync(user, vm.NewPassword!);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "No se pudo crear la contraseña. Intenta con otra.");
                return View(vm);
            }

            var stampResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "La contraseña fue creada, pero ocurrió un problema actualizando la seguridad de la cuenta.");
                return View(vm);
            }

            TempData["Success"] = "Contraseña creada. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KeepAlive()
        {
            // Endpoint utilizado por el frontend para mantener activa la sesión del usuario autenticado.
            return NoContent();
        }
    }
}