using ClosedXML.Excel;
using DataAccess;
using DataAccess.Modelos.Entidades.Integracion;
using DataAccess.Modelos.Entidades.Inventario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SASA.ViewModels.Integracion;

namespace SASA.Controllers
{
    public class IntegrationController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public IntegrationController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // Vista General
        public IActionResult Index() => View();

        // =========================
        // HU #61 + HU #64
        // =========================
        [HttpGet]
        public IActionResult Upload() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un archivo.");
                return View();
            }

            var ext = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (ext != ".xlsx")
            {
                ModelState.AddModelError("", "Formato inválido. Debe ser .xlsx");
                return View();
            }

            // Guardar el archivo
            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "integracion");
            Directory.CreateDirectory(carpeta);

            var nombreSeguro = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Path.GetFileName(archivo.FileName)}";
            var rutaFisica = Path.Combine(carpeta, nombreSeguro);

            await using (var stream = System.IO.File.Create(rutaFisica))
            {
                await archivo.CopyToAsync(stream);
            }

            // HU #64: crear historial
            var hist = new IntegracionHistorial
            {
                TipoProceso = "Importacion",
                Modulo = "Inventario",
                NombreArchivo = archivo.FileName,
                RutaArchivo = $"/uploads/integracion/{nombreSeguro}",
                Estado = "Cargado",
                Fecha = DateTime.UtcNow
            };

            _db.IntegracionHistorial.Add(hist);
            await _db.SaveChangesAsync();

            // HU #63: ir a validación
            return RedirectToAction(nameof(Validation), new { historialId = hist.Id });
        }

        // =========================
        // HU #63 + HU #64
        // =========================
        [HttpGet]
        public async Task<IActionResult> Validation(int historialId)
        {
            var hist = await _db.IntegracionHistorial.FirstOrDefaultAsync(h => h.Id == historialId);
            if (hist == null) return NotFound();

            var vm = new ValidacionImportacionViewModel
            {
                HistorialId = hist.Id,
                NombreArchivo = hist.NombreArchivo
            };

            try
            {
                var rutaFisica = Path.Combine(
                    _env.WebRootPath,
                    hist.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                // Requeridas (CORE Sprint 1)
                var requeridas = new[]
                {
                    "Numero de Activo",
                    "Nombre de maquina",
                    "Tipo de equipo"
                };

                using var wb = new XLWorkbook(rutaFisica);
                var ws = wb.Worksheets.First();

                // Header -> mapa
                var headerRow = ws.Row(1);
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (var cell in headerRow.CellsUsed())
                {
                    var name = cell.GetString().Trim();
                    if (!string.IsNullOrWhiteSpace(name))
                        headerMap[name] = cell.Address.ColumnNumber;
                }

                // Validar columnas requeridas
                var faltantes = requeridas.Where(r => !headerMap.ContainsKey(r)).ToList();
                if (faltantes.Any())
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = "Faltan columnas requeridas: " + string.Join(", ", faltantes);
                    await _db.SaveChangesAsync();

                    vm.Errores.Add(new FilaValidacion { NumeroFila = 0, Mensaje = hist.DetalleError });
                    return View(vm);
                }

                string GetStr(IXLRow row, string col)
                {
                    if (!headerMap.TryGetValue(col, out var idx)) return "";
                    return row.Cell(idx).GetString().Trim();
                }

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
                var codigosEnArchivo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // catálogos para validar tipo/licencia
                var tiposCatalogo = await _db.TipoActivoInventario.AsNoTracking()
                    .Select(t => t.Nombre).ToListAsync();
                var tiposSet = new HashSet<string>(tiposCatalogo, StringComparer.OrdinalIgnoreCase);

                var licCatalogo = await _db.TipoLicenciaInventario.AsNoTracking()
                    .Select(l => l.Nombre).ToListAsync();
                var licSet = new HashSet<string>(licCatalogo, StringComparer.OrdinalIgnoreCase);

                var filasValidas = new List<FilaImportacionActivos>();

                for (int r = 2; r <= lastRow; r++)
                {
                    var row = ws.Row(r);

                    var numeroActivo = GetStr(row, "Numero de Activo");
                    var nombreMaquina = GetStr(row, "Nombre de maquina");
                    var tipoEquipo = GetStr(row, "Tipo de equipo");

                    var marca = headerMap.ContainsKey("Marca") ? GetStr(row, "Marca") : null;
                    var modelo = headerMap.ContainsKey("Model") ? GetStr(row, "Model") : null;
                    var serie = headerMap.ContainsKey("Serie/Servitag") ? GetStr(row, "Serie/Servitag") : null;
                    var mac = headerMap.ContainsKey("MAC") ? GetStr(row, "MAC") : null;
                    var so = headerMap.ContainsKey("Sistema Operativo") ? GetStr(row, "Sistema Operativo") : null;

                    var tipoLic = headerMap.ContainsKey("Tipo de licencia") ? GetStr(row, "Tipo de licencia") : null;
                    var licencia = headerMap.ContainsKey("Licencia") ? GetStr(row, "Licencia") : null;

                    var erroresFila = new List<string>();

                    if (string.IsNullOrWhiteSpace(numeroActivo))
                        erroresFila.Add("Numero de Activo es requerido.");

                    if (string.IsNullOrWhiteSpace(nombreMaquina))
                        erroresFila.Add("Nombre de maquina es requerido.");

                    if (string.IsNullOrWhiteSpace(tipoEquipo))
                        erroresFila.Add("Tipo de equipo es requerido.");
                    else if (!tiposSet.Contains(tipoEquipo))
                        erroresFila.Add($"Tipo de equipo inválido: '{tipoEquipo}'.");

                    // duplicado dentro del Excel
                    if (!string.IsNullOrWhiteSpace(numeroActivo) && !codigosEnArchivo.Add(numeroActivo))
                        erroresFila.Add($"Numero de Activo duplicado en el Excel: '{numeroActivo}'.");

                    // duplicado contra BD
                    if (!string.IsNullOrWhiteSpace(numeroActivo))
                    {
                        var existeBD = await _db.ActivoInventario.AsNoTracking()
                            .AnyAsync(a => a.NumeroActivo == numeroActivo);

                        if (existeBD)
                            erroresFila.Add($"Ya existe en BD un activo con NumeroActivo '{numeroActivo}'.");
                    }

                    // licencia: si viene tipo licencia, validar
                    if (!string.IsNullOrWhiteSpace(tipoLic) && !licSet.Contains(tipoLic))
                        erroresFila.Add($"Tipo de licencia inválido: '{tipoLic}'.");

                    if (erroresFila.Any())
                    {
                        foreach (var err in erroresFila)
                            vm.Errores.Add(new FilaValidacion { NumeroFila = r, Mensaje = err });

                        continue;
                    }

                    filasValidas.Add(new FilaImportacionActivos
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

                // HU #64: actualizar historial
                hist.TotalFilas = vm.TotalFilas;
                hist.FilasValidas = vm.FilasValidas;
                hist.FilasConError = vm.FilasConError;
                hist.Estado = "Validado";
                hist.DetalleError = vm.Errores.Any()
                    ? $"Se detectaron {vm.FilasConError} filas con error."
                    : null;

                await _db.SaveChangesAsync();

                return View(vm);
            }
            catch (Exception ex)
            {
                hist.Estado = "Fallido";
                hist.DetalleError = ex.Message;
                await _db.SaveChangesAsync();

                vm.Errores.Add(new FilaValidacion { NumeroFila = 0, Mensaje = ex.Message });
                return View(vm);
            }
        }

        // =========================
        // HU #61 + HU #64
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmImport(int historialId)
        {
            var hist = await _db.IntegracionHistorial.FirstOrDefaultAsync(h => h.Id == historialId);
            if (hist == null) return NotFound();

            var rutaFisica = Path.Combine(
                _env.WebRootPath,
                hist.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

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

            // catálogos
            var tipos = await _db.TipoActivoInventario.AsNoTracking().ToListAsync();
            var estados = await _db.EstadoActivoInventario.AsNoTracking().ToListAsync();
            var licencias = await _db.TipoLicenciaInventario.AsNoTracking().ToListAsync();

            var idEstadoOperativo = estados.FirstOrDefault(e => e.Nombre == "Operativo")?.IdEstadoActivo ?? 1;

            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;
            int insertados = 0;
            int omitidos = 0;

            for (int r = 2; r <= lastRow; r++)
            {
                var row = ws.Row(r);

                var numeroActivo = GetStr(row, "Numero de Activo");
                var nombreMaquina = GetStr(row, "Nombre de maquina");
                var tipoEquipo = GetStr(row, "Tipo de equipo");

                if (string.IsNullOrWhiteSpace(numeroActivo) ||
                    string.IsNullOrWhiteSpace(nombreMaquina) ||
                    string.IsNullOrWhiteSpace(tipoEquipo))
                {
                    omitidos++;
                    continue;
                }

                // no duplicar
                if (await _db.ActivoInventario.AnyAsync(a => a.NumeroActivo == numeroActivo))
                {
                    omitidos++;
                    continue;
                }

                var tipo = tipos.FirstOrDefault(t => t.Nombre.Equals(tipoEquipo, StringComparison.OrdinalIgnoreCase));
                if (tipo == null)
                {
                    omitidos++;
                    continue;
                }

                var marca = headerMap.ContainsKey("Marca") ? GetStr(row, "Marca") : null;
                var modelo = headerMap.ContainsKey("Model") ? GetStr(row, "Model") : null;
                var serie = headerMap.ContainsKey("Serie/Servitag") ? GetStr(row, "Serie/Servitag") : null;
                var mac = headerMap.ContainsKey("MAC") ? GetStr(row, "MAC") : null;
                var so = headerMap.ContainsKey("Sistema Operativo") ? GetStr(row, "Sistema Operativo") : null;

                var tipoLic = headerMap.ContainsKey("Tipo de licencia") ? GetStr(row, "Tipo de licencia") : null;
                var licenciaKey = headerMap.ContainsKey("Licencia") ? GetStr(row, "Licencia") : null;

                int? idTipoLic = null;
                if (!string.IsNullOrWhiteSpace(tipoLic))
                {
                    var lic = licencias.FirstOrDefault(l => l.Nombre.Equals(tipoLic, StringComparison.OrdinalIgnoreCase));
                    if (lic != null) idTipoLic = lic.IdTipoLicencia;
                }

                var activo = new ActivoInventario
                {
                    NumeroActivo = numeroActivo,
                    NombreMaquina = nombreMaquina,
                    IdTipoActivo = tipo.IdTipoActivo,
                    IdEstadoActivo = idEstadoOperativo, // default Sprint 1
                    Marca = marca,
                    Modelo = modelo,
                    SerieServicio = serie,
                    DireccionMAC = mac,
                    SistemaOperativo = so,
                    IdTipoLicencia = idTipoLic,
                    ClaveLicencia = licenciaKey,
                    FechaCreacion = DateTime.UtcNow
                };

                _db.ActivoInventario.Add(activo);
                insertados++;
            }

            await _db.SaveChangesAsync();

            // HU #64: actualizar historial
            hist.Estado = "Importado";
            hist.DetalleError = $"Insertados: {insertados}. Omitidos: {omitidos}.";
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(History));
        }

        // =========================
        // HU #64
        // =========================
        [HttpGet]
        public async Task<IActionResult> History()
        {
            var data = await _db.IntegracionHistorial
                .AsNoTracking()
                .OrderByDescending(h => h.Fecha)
                .ToListAsync();

            return View(data);
        }
    }
}
