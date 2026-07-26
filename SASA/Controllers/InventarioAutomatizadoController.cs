using BusinessLogic.Servicios.Inventario;
using DataAccess.Modelos.DTOs.Inventario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SASA.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace SASA.Controllers
{
    [ApiController]
    [Route("api/inventario-automatizado")]
    public class InventarioAutomatizadoController : ControllerBase
    {
        private readonly IInventarioAutomatizadoService _service;
        private readonly InventarioAutomatizadoSettings _settings;
        private readonly ILogger<InventarioAutomatizadoController> _logger;

        public InventarioAutomatizadoController(
            IInventarioAutomatizadoService service,
            IOptions<InventarioAutomatizadoSettings> settings,
            ILogger<InventarioAutomatizadoController> logger)
        {
            _service = service;
            _settings = settings.Value;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Registrar(
            [FromBody] InventarioAutomatizadoRequestDto request)
        {
            if (!Request.Headers.TryGetValue(
                    "X-SASA-API-KEY",
                    out var apiKeyRecibida))
            {
                _logger.LogWarning(
                    "Solicitud de inventario automatizado rechazada: API key ausente. IP: {Ip}",
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                return Unauthorized(new
                {
                    ok = false,
                    mensaje = "No se proporcionó el código de acceso."
                });
            }

            if (!ApiKeysCoinciden(
                    apiKeyRecibida.ToString(),
                    _settings.ApiKey))
            {
                _logger.LogWarning(
                    "Solicitud de inventario automatizado rechazada: API key inválida. IP: {Ip}",
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                return Unauthorized(new
                {
                    ok = false,
                    mensaje = "El código de acceso es inválido."
                });
            }

            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray());

                return BadRequest(new
                {
                    ok = false,
                    mensaje = "Los datos enviados no son válidos.",
                    errores
                });
            }

            var resultado = await _service.RegistrarAsync(request);

            if (!resultado.Ok)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ok = false,
                        mensaje = resultado.Mensaje
                    });
            }

            return Ok(new
            {
                ok = true,
                mensaje = resultado.Mensaje,
                idInventarioAutomatizado = resultado.Id,
                actualizado = resultado.Actualizado
            });
        }

        private static bool ApiKeysCoinciden(
            string recibida,
            string configurada)
        {
            if (string.IsNullOrWhiteSpace(recibida) ||
                string.IsNullOrWhiteSpace(configurada))
            {
                return false;
            }

            var recibidaBytes = Encoding.UTF8.GetBytes(recibida);
            var configuradaBytes = Encoding.UTF8.GetBytes(configurada);

            if (recibidaBytes.Length != configuradaBytes.Length)
                return false;

            return CryptographicOperations.FixedTimeEquals(
                recibidaBytes,
                configuradaBytes);
        }
    }
}