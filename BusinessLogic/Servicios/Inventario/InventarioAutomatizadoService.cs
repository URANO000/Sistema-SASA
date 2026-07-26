using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Modelos.Entidades.Inventario;
using DataAccess.Repositorios.Inventario;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace BusinessLogic.Servicios.Inventario
{
    public class InventarioAutomatizadoService
        : IInventarioAutomatizadoService
    {
        private readonly IInventarioAutomatizadoRepository _repository;
        private readonly IActivoInventarioRepository _activoRepository;
        private readonly ICatalogosInventarioRepository _catalogosRepository;
        private readonly ILogger<InventarioAutomatizadoService> _logger;

        public InventarioAutomatizadoService(
            IInventarioAutomatizadoRepository repository,
            IActivoInventarioRepository activoRepository,
            ICatalogosInventarioRepository catalogosRepository,
            ILogger<InventarioAutomatizadoService> logger)
        {
            _repository = repository;
            _activoRepository = activoRepository;
            _catalogosRepository = catalogosRepository;
            _logger = logger;
        }

        public async Task<(bool Ok, string Mensaje, int? Id, bool Actualizado)>
            RegistrarAsync(InventarioAutomatizadoRequestDto request)
        {
            try
            {
                var nombreEquipo =
                    NormalizarRequerido(request.NombreEquipo);

                var serialPC =
                    Normalizar(request.SerialPC);

                var fechaActual = DateTime.UtcNow;

                /*
                 * 1. Guardar o actualizar la información técnica
                 *    completa proveniente de HawkEye.
                 */
                var existente = await _repository.ObtenerExistenteAsync(
                    serialPC,
                    nombreEquipo);

                var actualizado = existente != null;

                if (existente == null)
                {
                    existente = new InventarioAutomatizado
                    {
                        FechaRegistro = fechaActual
                    };

                    await _repository.AgregarAsync(existente);
                }

                existente.NombreUsuario =
                    Normalizar(request.NombreUsuario);

                existente.NombreEquipo =
                    nombreEquipo;

                existente.Marca =
                    Normalizar(request.Marca);

                existente.Modelo =
                    Normalizar(request.Modelo);

                existente.SerialPC =
                    serialPC;

                existente.ModeloProcesador =
                    Normalizar(request.ModeloProcesador);

                existente.Mhz =
                    Normalizar(request.Mhz);

                existente.SistemaOperativo =
                    Normalizar(request.So);

                existente.BuildNumber =
                    Normalizar(request.BuildNumber);

                existente.TotalRam =
                    request.TotalRam;

                existente.SlotsRam =
                    request.SlotsRam;

                existente.SlotDetalle =
                    NormalizarTextoLargo(request.SlotDetalle);

                existente.ModeloAlmacenamiento =
                    Normalizar(request.ModeloAlmacenamiento);

                existente.AlmacenamientoDetalle =
                    NormalizarTextoLargo(
                        request.AlmacenamientoDetalle);

                existente.Redes =
                    NormalizarTextoLargo(request.Redes);

                existente.RedDetalle =
                    NormalizarTextoLargo(request.RedDetalle);

                existente.CantMonitores =
                    request.CantMonitores;

                existente.Monitores =
                    NormalizarTextoLargo(request.Monitores);

                existente.MonitorDetalle =
                    NormalizarTextoLargo(
                        request.MonitorDetalle);

                existente.Tiene7 =
                    request.Tiene7;

                existente.Tiene6 =
                    request.Tiene6;

                existente.TieneKonica =
                    request.TieneKonica;

                existente.FechaActualizacion =
                    fechaActual;

                /*
                 * 2. Crear o actualizar el registro oficial dentro
                 *    del Inventario de Equipos de SASA.
                 */
                await SincronizarActivoInventarioAsync(
                    request,
                    nombreEquipo,
                    serialPC,
                    fechaActual);

                /*
                 * Ambos repositorios utilizan el mismo DbContext
                 * dentro de la solicitud, por lo que este guardado
                 * persiste los cambios pendientes.
                 */
                await _repository.GuardarAsync();

                var mensaje = actualizado
                    ? "La información del equipo fue actualizada correctamente."
                    : "La información del equipo fue registrada correctamente.";

                _logger.LogInformation(
                    "Inventario automatizado procesado. Id: {Id}, Equipo: {Equipo}, Serial: {Serial}, Actualizado: {Actualizado}",
                    existente.IdInventarioAutomatizado,
                    existente.NombreEquipo,
                    existente.SerialPC,
                    actualizado);

                return (
                    true,
                    mensaje,
                    existente.IdInventarioAutomatizado,
                    actualizado
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error procesando inventario automatizado. Equipo: {Equipo}, Serial: {Serial}",
                    request.NombreEquipo,
                    request.SerialPC);

                return (
                    false,
                    "No se pudo procesar la información del equipo.",
                    null,
                    false
                );
            }
        }

        private async Task SincronizarActivoInventarioAsync(
            InventarioAutomatizadoRequestDto request,
            string nombreEquipo,
            string serialPC,
            DateTime fechaActual)
        {
            var activo = await _activoRepository
                .ObtenerPorSerieONombreAsync(
                    serialPC,
                    nombreEquipo);

            var direccionMac = ExtraerPrimeraMac(
                request.RedDetalle,
                request.Redes);

            /*
             * Si el activo ya existe, solamente se actualizan
             * los datos técnicos obtenidos por HawkEye.
             *
             * No se modifica:
             * - NumeroActivo
             * - IdTipoActivo
             * - IdEstadoActivo
             * - IdTipoLicencia
             * - ClaveLicencia
             * - UsuarioActualId
             * - UsuarioAnteriorId
             */
            if (activo != null)
            {
                activo.NombreMaquina =
                    nombreEquipo;

                activo.Marca =
                    Normalizar(request.Marca);

                activo.Modelo =
                    Normalizar(request.Modelo);

                activo.SerieServicio =
                    serialPC;

                activo.DireccionMAC =
                    direccionMac;

                activo.SistemaOperativo =
                    Normalizar(request.So);

                activo.FechaActualizacion =
                    fechaActual;

                return;
            }

            /*
             * El activo no existe: localizar tipo y estado
             * por nombre, nunca mediante IDs fijos.
             */
            var tipos = await _catalogosRepository
                .ObtenerTiposAsync();

            var estados = await _catalogosRepository
                .ObtenerEstadosAsync();

            var nombreTipoActivo = DeterminarTipoActivo(
                request.Marca,
                request.Modelo,
                nombreEquipo);

            var tipoActivo = tipos.FirstOrDefault(x =>
                string.Equals(
                    x.Nombre?.Trim(),
                    nombreTipoActivo,
                    StringComparison.OrdinalIgnoreCase));

            var estadoActivo = estados.FirstOrDefault(x =>
                string.Equals(
                    x.Nombre?.Trim(),
                    "Operativo",
                    StringComparison.OrdinalIgnoreCase));

            if (tipoActivo == null)
            {
                throw new InvalidOperationException(
                    $"No se encontró en el catálogo el tipo de activo '{nombreTipoActivo}'.");
            }

            if (estadoActivo == null)
            {
                throw new InvalidOperationException(
                    "No se encontró en el catálogo el estado 'Operativo'.");
            }



            var numeroActivo = GenerarNumeroActivo(
                serialPC,
                nombreEquipo);

            /*
             * Evitar una posible colisión del código generado.
             */
            var numeroBase = numeroActivo;
            var consecutivo = 1;

            while (await _activoRepository
                       .ExisteNumeroActivoAsync(numeroActivo))
            {
                numeroActivo =
                    $"{numeroBase}-{consecutivo}";

                consecutivo++;
            }

            var nuevoActivo = new ActivoInventario
            {
                NumeroActivo = numeroActivo,
                NombreMaquina = nombreEquipo,
                Marca = Normalizar(request.Marca),
                Modelo = Normalizar(request.Modelo),
                SerieServicio = serialPC,
                DireccionMAC = direccionMac,
                SistemaOperativo = Normalizar(request.So),

                IdTipoActivo =
                    tipoActivo.IdTipoActivo,

                IdEstadoActivo =
                    estadoActivo.IdEstadoActivo,

                IdTipoLicencia = null,
                ClaveLicencia = null,

                FechaCreacion = fechaActual
            };

            await _activoRepository.CrearAsync(nuevoActivo);
        }

        private static string DeterminarTipoActivo(
            string? marca,
            string? modelo,
            string? nombreEquipo)
        {
            var texto = string.Join(
                " ",
                marca ?? string.Empty,
                modelo ?? string.Empty,
                nombreEquipo ?? string.Empty)
                .ToUpperInvariant();

            /*
             * Servidores
             */
            if (texto.Contains("SERVER") ||
                texto.Contains("SERVIDOR") ||
                texto.Contains("POWEREDGE") ||
                texto.Contains("PROLIANT") ||
                texto.Contains("THINKSYSTEM") ||
                texto.Contains("PRIMERGY"))
            {
                return "Servidor";
            }

            /*
             * Computadoras portátiles
             */
            if (texto.Contains("LAPTOP") ||
                texto.Contains("NOTEBOOK") ||
                texto.Contains("PORTATIL") ||
                texto.Contains("PORTÁTIL") ||
                texto.Contains("THINKPAD") ||
                texto.Contains("LATITUDE") ||
                texto.Contains("ELITEBOOK") ||
                texto.Contains("PROBOOK") ||
                texto.Contains("PAVILION") ||
                texto.Contains("ASPIRE") ||
                texto.Contains("IDEAPAD") ||
                texto.Contains("VIVOBOOK") ||
                texto.Contains("ZENBOOK") ||
                texto.Contains("MACBOOK"))
            {
                return "Laptop";
            }

            /*
             * Si no se identifica como portátil o servidor,
             * se clasifica como computadora de escritorio.
             */
            return "Desktop";
        }
        private static string GenerarNumeroActivo(
            string serialPC,
            string nombreEquipo)
        {
            var identificador =
                !string.IsNullOrWhiteSpace(serialPC)
                    ? serialPC
                    : nombreEquipo;

            identificador = Regex.Replace(
                identificador,
                @"[^A-Za-z0-9\-]",
                string.Empty);

            if (string.IsNullOrWhiteSpace(identificador))
            {
                identificador =
                    Guid.NewGuid()
                        .ToString("N")[..12]
                        .ToUpperInvariant();
            }

            return $"AUTO-{identificador.ToUpperInvariant()}";
        }

        private static string ExtraerPrimeraMac(
            string? redDetalle,
            string? redes)
        {
            var texto = !string.IsNullOrWhiteSpace(redDetalle)
                ? redDetalle
                : redes;

            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var coincidencia = Regex.Match(
                texto,
                @"(?:[0-9A-Fa-f]{2}[:-]){5}[0-9A-Fa-f]{2}");

            return coincidencia.Success
                ? coincidencia.Value.ToUpperInvariant()
                : string.Empty;
        }

        private static string Normalizar(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return string.Join(
                " ",
                valor.Trim()
                    .Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries));
        }

        private static string NormalizarRequerido(
            string? valor)
        {
            return Normalizar(valor);
        }

        private static string NormalizarTextoLargo(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? string.Empty
                : valor.Trim();
        }
    }
}