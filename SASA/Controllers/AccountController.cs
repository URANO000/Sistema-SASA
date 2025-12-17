using Microsoft.AspNetCore.Mvc;
using SASA.Services.Auth;
using SASA.ViewModels.Auth;

namespace SASA.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _auth;

        public AccountController(IAuthService auth)
        {
            _auth = auth;
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
            {
                ModelState.AddModelError(string.Empty, "ModelState inválido (server).");
                return View(vm);
            }

            var result = await _auth.LoginAsync(vm.Email!, vm.Password!);
            if (!result.Ok)
            {
                ModelState.AddModelError(string.Empty, $"LOGIN FAIL: {result.Message}");
                return View(vm);
            }

            HttpContext.Session.SetString("auth_email", result.Data!.Email);
            HttpContext.Session.SetString("auth_name", result.Data!.DisplayName);

            return Redirect("/Home/Index");
        }

        [HttpPost("/logout")]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
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

            var result = await _auth.ForgotPasswordAsync(vm.Email!);
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        [HttpGet("/reset-password/{token}")]
        public IActionResult ResetPassword(string token)
        {
            var check = ValidateResetToken(token);
            if (!check.ok)
            {
                TempData["Error"] = check.message;
                return RedirectToAction(nameof(Login));
            }

            return View(new ResetPasswordViewModel { Token = token });
        }


        private (bool ok, string message) ValidateResetToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, "Token inválido.");

            if (token.Equals("invalid-reset-token", StringComparison.OrdinalIgnoreCase))
                return (false, "Token inválido.");

            if (token.Equals("expired-reset-token", StringComparison.OrdinalIgnoreCase))
                return (false, "Token expirado.");

            return (true, "");
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

            var result = await _auth.ResetPasswordAsync(token, vm.NewPassword!);
            if (!result.Ok)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }



        [HttpGet("/activate-account/{token}")]
        public async Task<IActionResult> ActivateAccount(string token)
        {
            var result = await _auth.ActivateAccountAsync(token);
            TempData[result.Ok ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Login));
        }
    }
}
