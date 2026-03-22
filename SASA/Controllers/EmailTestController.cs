using BusinessLogic.Servicios.Correo;
using Microsoft.AspNetCore.Mvc;
using SASA.Filters;

namespace SASA.Controllers
{
    [RequireAuth]
    [Route("dev/email-test")]
    public class EmailTestController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public EmailTestController(IEmailService emailService, IWebHostEnvironment env)
        {
            _emailService = emailService;
            _env = env;
        }

        // GET /dev/email-test
        [HttpGet("")]
        public IActionResult Index()
        {
            if (!_env.IsDevelopment())
                return NotFound();

            return View();
        }

        // POST /dev/email-test/send
        [HttpPost("send")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string toEmail)
        {
            if (!_env.IsDevelopment())
                return NotFound();

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                ModelState.AddModelError(nameof(toEmail), "Debes ingresar un correo destino.");
                return View("Index");
            }

            try
            {
                await _emailService.SendEmailAsync(
                    toEmail: toEmail,
                    toName: "Prueba SASA",
                    subject: "Prueba de correo - Microsoft Graph (SASA)",
                    htmlBody: "<p>Correo de prueba enviado desde SASA usando Microsoft Graph.</p>"
                );

                TempData["Success"] =
                    $"Solicitud de envío realizada para {toEmail}. Revisa spam o cuarentena.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.ToString();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
