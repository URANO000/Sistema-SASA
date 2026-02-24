using ClosedXML.Excel;
using DataAccess;
using DataAccess.Modelos.DTOs.Integracion;
using DataAccess.Modelos.Entidades.Integracion;
using DataAccess.Modelos.Entidades.Inventario;
using DataAccess.Repositorios.Integracion;
using DataAccess.Repositorios.Inventario;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Servicios.Integracion
{
    public class IntegracionService : IIntegracionService
    {
        private readonly IIntegracionHistorialRepository _histRepo;
        private readonly IActivoInventarioRepository _activoRepo;
        private readonly ICatalogosInventarioRepository _catRepo;
        private readonly ApplicationDbContext _db; // solo para transacciones

        public IntegracionService(
            IIntegracionHistorialRepository histRepo,
            IActivoInventarioRepository activoRepo,
            ICatalogosInventarioRepository catRepo,
            ApplicationDbContext db)
        {
            _histRepo = histRepo;
            _activoRepo = activoRepo;
            _catRepo = catRepo;
            _db = db;
        }

        public Task<List<IntegracionHistorial>> ObtenerHistorialAsync() => _histRepo.ListarAsync();

        public async Task<int> RegistrarCargaAsync(string nombreArchivoOriginal, string rutaArchivoRelativa, string? usuarioId)
        {
            var hist = new IntegracionHistorial
            {
                TipoProceso = "Importacion",
                Modulo = "Inventario",
                NombreArchivo = nombreArchivoOriginal,
                RutaArchivo = rutaArchivoRelativa,
                Estado = "Cargado",
                Fecha = DateTime.UtcNow,
                UsuarioEjecutorId = usuarioId
            };

            return await _histRepo.CrearAsync(hist);
        }

        public async Task<ValidacionImportacionDto> ValidarInventarioDesdeExcelAsync(int historialId, string webRootPath)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null) throw new InvalidOperationException("Historial no encontrado.");

            var vm = new ValidacionImportacionDto
            {
                HistorialId = hist.Id,
                NombreArchivo = hist.NombreArchivo
            };

            var rutaFisica = Path.Combine(
                webRootPath,
                hist.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            // Requeridas (CORE Sprint 1)
            var requeridas = new[] { "Numero de Activo", "Nombre de maquina", "Tipo de equipo" };

            try
            {
                using var wb = new XLWorkbook(rutaFisica);
                var ws = wb.Worksheets.First();

                // Header -> mapa (con control de duplicados)
                var headerRow = ws.Row(1);
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (var cell in headerRow.CellsUsed())
                {
                    var name = cell.GetString().Trim();
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    if (headerMap.ContainsKey(name))
                        throw new InvalidOperationException($"Encabezado duplicado en Excel: '{name}'.");

                    headerMap[name] = cell.Address.ColumnNumber;
                }

                // Validar columnas requeridas
                var faltantes = requeridas.Where(r => !headerMap.ContainsKey(r)).ToList();
                if (faltantes.Any())
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = "Faltan columnas requeridas: " + string.Join(", ", faltantes);
                    await _histRepo.GuardarAsync();

                    vm.Errores.Add(new FilaValidacionDto { NumeroFila = 0, Mensaje = hist.DetalleError });
                    return vm;
                }

                string GetStr(IXLRow row, string col)
                {
                    if (!headerMap.TryGetValue(col, out var idx)) return "";
                    return row.Cell(idx).GetString().Trim();
                }

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

                // ✅ NUEVO: archivo sin filas de datos
                if (lastRow < 2)
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = "El archivo no contiene filas de datos.";
                    await _histRepo.GuardarAsync();

                    vm.Errores.Add(new FilaValidacionDto { NumeroFila = 0, Mensaje = hist.DetalleError });
                    return vm;
                }

                var codigosEnArchivo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // catálogos
                var tiposSet = await _catRepo.ObtenerTiposSetAsync();
                var licSet = await _catRepo.ObtenerLicenciasSetAsync();

                // === OPTIMIZACIÓN: 1 sola consulta a BD ===
                var numerosArchivo = new List<string>();
                for (int r = 2; r <= lastRow; r++)
                {
                    var numero = GetStr(ws.Row(r), "Numero de Activo");
                    if (!string.IsNullOrWhiteSpace(numero))
                        numerosArchivo.Add(numero.Trim());
                }

                var existentesBd = await _activoRepo.ObtenerNumerosExistentesAsync(numerosArchivo);
                var existentesSet = new HashSet<string>(existentesBd, StringComparer.OrdinalIgnoreCase);
                // =========================================

                var filasValidas = new List<FilaImportacionActivosDto>();

                for (int r = 2; r <= lastRow; r++)
                {
                    var row = ws.Row(r);

                    var numeroActivo = Normalize(GetStr(row, "Numero de Activo"));
                    var nombreMaquina = Normalize(GetStr(row, "Nombre de maquina"));
                    var tipoEquipo = Normalize(GetStr(row, "Tipo de equipo"));

                    var marca = headerMap.ContainsKey("Marca") ? Normalize(GetStr(row, "Marca")) : null;
                    var modelo = headerMap.ContainsKey("Model") ? Normalize(GetStr(row, "Model")) : null;
                    var serie = headerMap.ContainsKey("Serie/Servitag") ? Normalize(GetStr(row, "Serie/Servitag")) : null;
                    var mac = headerMap.ContainsKey("MAC") ? Normalize(GetStr(row, "MAC")) : null;
                    var so = headerMap.ContainsKey("Sistema Operativo") ? Normalize(GetStr(row, "Sistema Operativo")) : null;

                    var tipoLic = headerMap.ContainsKey("Tipo de licencia") ? Normalize(GetStr(row, "Tipo de licencia")) : null;
                    var licencia = headerMap.ContainsKey("Licencia") ? Normalize(GetStr(row, "Licencia")) : null;

                    var errores = new List<string>();

                    if (string.IsNullOrWhiteSpace(numeroActivo)) errores.Add("Numero de Activo es requerido.");
                    if (string.IsNullOrWhiteSpace(nombreMaquina)) errores.Add("Nombre de maquina es requerido.");

                    if (string.IsNullOrWhiteSpace(tipoEquipo))
                        errores.Add("Tipo de equipo es requerido.");
                    else if (!tiposSet.Contains(tipoEquipo))
                        errores.Add($"Tipo de equipo inválido: '{tipoEquipo}'.");

                    if (!string.IsNullOrWhiteSpace(numeroActivo) && !codigosEnArchivo.Add(numeroActivo))
                        errores.Add($"Numero de Activo duplicado en el Excel: '{numeroActivo}'.");

                    if (!string.IsNullOrWhiteSpace(numeroActivo) && existentesSet.Contains(numeroActivo))
                        errores.Add($"Ya existe en BD un activo con NumeroActivo '{numeroActivo}'.");

                    if (!string.IsNullOrWhiteSpace(tipoLic) && !licSet.Contains(tipoLic))
                        errores.Add($"Tipo de licencia inválido: '{tipoLic}'.");

                    // HU62: Reglas de consistencia y calidad de datos
                    if (!string.IsNullOrWhiteSpace(numeroActivo) && !IsValidNumeroActivo(numeroActivo))
                        errores.Add("Numero de Activo tiene formato inválido (solo letras/números y - _ .; 3 a 40 chars).");

                    if (!string.IsNullOrWhiteSpace(nombreMaquina) && nombreMaquina.Length > 80)
                        errores.Add("Nombre de maquina excede el máximo permitido (80 caracteres).");

                    if (!string.IsNullOrWhiteSpace(mac) && !IsValidMac(mac))
                        errores.Add("MAC tiene formato inválido.");

                    if (!string.IsNullOrWhiteSpace(so) && so.Length > 80)
                        errores.Add("Sistema Operativo excede el máximo permitido (80 caracteres).");

                    if (!string.IsNullOrWhiteSpace(licencia) && string.IsNullOrWhiteSpace(tipoLic))
                        errores.Add("Si se indica Licencia, también debe indicarse Tipo de licencia.");

                    if (errores.Any())
                    {
                        foreach (var e in errores)
                            vm.Errores.Add(new FilaValidacionDto { NumeroFila = r, Mensaje = e });
                        continue;
                    }

                    filasValidas.Add(new FilaImportacionActivosDto
                    {
                        NumeroFila = r,
                        NumeroActivo = numeroActivo,
                        NombreMaquina = nombreMaquina,
                        TipoEquipo = tipoEquipo,
                        Marca = marca,
                        Modelo = modelo,
                        SerieServicio = serie,
                        DireccionMAC = mac,
                        SistemaOperativo = so,
                        TipoLicencia = tipoLic,
                        ClaveLicencia = licencia
                    });
                }

                vm.TotalFilas = Math.Max(0, lastRow - 1);
                vm.FilasValidas = filasValidas.Count;
                vm.FilasConError = vm.TotalFilas - vm.FilasValidas;
                vm.FilasValidasPreview = filasValidas.Take(20).ToList();

                hist.TotalFilas = vm.TotalFilas;
                hist.FilasValidas = vm.FilasValidas;
                hist.FilasConError = vm.FilasConError;
                hist.Estado = "Validado";

                // ✅ NUEVO: detalle más informativo
                hist.DetalleError = vm.Errores.Any()
                    ? $"Errores detectados: {vm.FilasConError}. Total válidas: {vm.FilasValidas}."
                    : null;

                await _histRepo.GuardarAsync();
                return vm;
            }
            catch (Exception ex)
            {
                hist.Estado = "Fallido";
                hist.DetalleError = ex.Message;
                await _histRepo.GuardarAsync();

                vm.Errores.Add(new FilaValidacionDto { NumeroFila = 0, Mensaje = ex.Message });
                return vm;
            }
        }

        public async Task ConfirmarImportacionInventarioAsync(int historialId, string webRootPath, string? usuarioId)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null) throw new InvalidOperationException("Historial no encontrado.");

            hist.UsuarioEjecutorId ??= usuarioId;

            var rutaFisica = Path.Combine(
                webRootPath,
                hist.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                using var wb = new XLWorkbook(rutaFisica);
                var ws = wb.Worksheets.First();

                // Header map
                var headerRow = ws.Row(1);
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (var cell in headerRow.CellsUsed())
                {
                    var name = cell.GetString().Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                        headerMap[name] = cell.Address.ColumnNumber;
                }

                string GetStr(IXLRow row, string col)
                {
                    if (!headerMap.TryGetValue(col, out var idx)) return "";
                    return row.Cell(idx).GetString().Trim();
                }

                var tipos = await _catRepo.ObtenerTiposAsync();
                var estados = await _catRepo.ObtenerEstadosAsync();
                var licencias = await _catRepo.ObtenerLicenciasAsync();

                var idEstadoOperativo = estados.FirstOrDefault(e => e.Nombre == "Operativo")?.IdEstadoActivo ?? 1;

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

                // evitar Any por fila (1 consulta)
                var numerosArchivo = new List<string>();
                for (int r = 2; r <= lastRow; r++)
                {
                    var n = GetStr(ws.Row(r), "Numero de Activo");
                    if (!string.IsNullOrWhiteSpace(n)) numerosArchivo.Add(n.Trim());
                }

                var existentesBd = await _activoRepo.ObtenerNumerosExistentesAsync(numerosArchivo);
                var existentesSet = new HashSet<string>(existentesBd, StringComparer.OrdinalIgnoreCase);

                int insertados = 0, omitidos = 0;
                var nuevos = new List<ActivoInventario>();

                for (int r = 2; r <= lastRow; r++)
                {
                    var row = ws.Row(r);

                    var numeroActivo = Normalize(GetStr(row, "Numero de Activo"));
                    var nombreMaquina = Normalize(GetStr(row, "Nombre de maquina"));
                    var tipoEquipo = Normalize(GetStr(row, "Tipo de equipo"));

                    if (string.IsNullOrWhiteSpace(numeroActivo) ||
                        string.IsNullOrWhiteSpace(nombreMaquina) ||
                        string.IsNullOrWhiteSpace(tipoEquipo))
                    { omitidos++; continue; }

                    if (existentesSet.Contains(numeroActivo))
                    { omitidos++; continue; }

                    var tipo = tipos.FirstOrDefault(t => t.Nombre.Equals(tipoEquipo, StringComparison.OrdinalIgnoreCase));
                    if (tipo == null)
                    { omitidos++; continue; }

                    var marca = headerMap.ContainsKey("Marca") ? GetStr(row, "Marca") : null;
                    var modelo = headerMap.ContainsKey("Model") ? GetStr(row, "Model") : null;
                    var serie = headerMap.ContainsKey("Serie/Servitag") ? GetStr(row, "Serie/Servitag") : null;
                    var mac = headerMap.ContainsKey("MAC") ? Normalize(GetStr(row, "MAC")) : null;
                    var so = headerMap.ContainsKey("Sistema Operativo") ? Normalize(GetStr(row, "Sistema Operativo")) : null;

                    var tipoLic = headerMap.ContainsKey("Tipo de licencia") ? Normalize(GetStr(row, "Tipo de licencia")) : null;
                    var licenciaKey = headerMap.ContainsKey("Licencia") ? Normalize(GetStr(row, "Licencia")) : null;

                    int? idTipoLic = null;
                    if (!string.IsNullOrWhiteSpace(tipoLic))
                    {
                        var lic = licencias.FirstOrDefault(l => l.Nombre.Equals(tipoLic, StringComparison.OrdinalIgnoreCase));
                        if (lic != null) idTipoLic = lic.IdTipoLicencia;
                    }

                    // ✅ Validación defensiva (HU62) en Confirmación
                    if (!IsValidNumeroActivo(numeroActivo)) { omitidos++; continue; }
                    if (!string.IsNullOrWhiteSpace(mac) && !IsValidMac(mac)) { omitidos++; continue; }
                    if (!string.IsNullOrWhiteSpace(licenciaKey) && string.IsNullOrWhiteSpace(tipoLic)) { omitidos++; continue; }

                    nuevos.Add(new ActivoInventario
                    {
                        NumeroActivo = numeroActivo,
                        NombreMaquina = nombreMaquina,
                        IdTipoActivo = tipo.IdTipoActivo,
                        IdEstadoActivo = idEstadoOperativo,
                        Marca = marca,
                        Modelo = modelo,
                        SerieServicio = serie,
                        DireccionMAC = mac,
                        SistemaOperativo = so,
                        IdTipoLicencia = idTipoLic,
                        ClaveLicencia = licenciaKey,
                        FechaCreacion = DateTime.UtcNow
                    });

                    existentesSet.Add(numeroActivo);
                    insertados++;
                }

                // ✅ NUEVO: si no hay cambios, guardar estado más explícito
                if (nuevos.Count == 0)
                {
                    hist.Estado = "SinCambios";
                    hist.DetalleError = "No se insertaron registros.";
                    await _histRepo.GuardarAsync();

                    await tx.CommitAsync();
                    return;
                }

                await _activoRepo.AgregarRangoAsync(nuevos);
                await _activoRepo.GuardarAsync();

                hist.Estado = "Importado";
                hist.DetalleError = $"Insertados: {insertados}. Omitidos: {omitidos}.";
                await _histRepo.GuardarAsync();

                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                hist.Estado = "Fallido";
                hist.DetalleError = ex.Message;
                await _histRepo.GuardarAsync();
                throw;
            }
        }

        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return string.Join(" ", value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private static bool IsValidNumeroActivo(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (value.Length < 3 || value.Length > 40) return false;

            foreach (var ch in value)
            {
                if (!(char.IsLetterOrDigit(ch) || ch == '-' || ch == '_' || ch == '.'))
                    return false;
            }
            return true;
        }

        private static bool IsValidMac(string value)
        {
            var s = value.Trim();
            string compact = s.Replace(":", "").Replace("-", "");
            if (compact.Length != 12) return false;

            foreach (var ch in compact)
            {
                bool hex = (ch >= '0' && ch <= '9') ||
                           (ch >= 'a' && ch <= 'f') ||
                           (ch >= 'A' && ch <= 'F');
                if (!hex) return false;
            }

            return true;
        }
    }
}