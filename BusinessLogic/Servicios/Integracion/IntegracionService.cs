using ClosedXML.Excel;
using BusinessLogic.Servicios.InventarioTelefono;
using DataAccess.Modelos.DTOs.InventarioTelefono;
using DataAccess;
using DataAccess.Modelos.DTOs.Integracion;
using DataAccess.Modelos.Entidades.Integracion;
using DataAccess.Modelos.Entidades.Inventario;
using DataAccess.Repositorios.Integracion;
using DataAccess.Repositorios.Inventario;

namespace BusinessLogic.Servicios.Integracion
{
    public class IntegracionService : IIntegracionService
    {
        private readonly IIntegracionHistorialRepository _histRepo;
        private readonly IActivoInventarioRepository _activoRepo;
        private readonly IActivoTelefonoService _telefonoService;
        private readonly ICatalogosInventarioRepository _catRepo;
        private readonly ApplicationDbContext _db;

        public IntegracionService(
            IIntegracionHistorialRepository histRepo,
            IActivoInventarioRepository activoRepo,
            ICatalogosInventarioRepository catRepo,
            IActivoTelefonoService telefonoService,
            ApplicationDbContext db)
        {
            _histRepo = histRepo;
            _activoRepo = activoRepo;
            _catRepo = catRepo;
            _telefonoService = telefonoService;
            _db = db;
        }

        public Task<List<IntegracionHistorial>> ObtenerHistorialAsync() => _histRepo.ListarAsync();

        public async Task<int> RegistrarCargaAsync(string nombreArchivoOriginal, string rutaArchivoRelativa, string? usuarioId, string tipoCarga)
        {
            var modulo = tipoCarga == "Telefonos"
                ? "InventarioTelefonos"
                : "InventarioEquipos";

            var hist = new IntegracionHistorial
            {
                TipoProceso = "Importacion",
                Modulo = modulo,
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

            var requeridas = new[] { "Numero de Activo", "Nombre de maquina", "Tipo de equipo" };

            try
            {
                using var wb = new XLWorkbook(rutaFisica);
                var ws = wb.Worksheets.First();

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

                if (lastRow < 2)
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = "El archivo no contiene filas de datos.";
                    await _histRepo.GuardarAsync();

                    vm.Errores.Add(new FilaValidacionDto { NumeroFila = 0, Mensaje = hist.DetalleError });
                    return vm;
                }

                var tipos = await _catRepo.ObtenerTiposAsync();
                var licencias = await _catRepo.ObtenerLicenciasAsync();

                vm.TiposEquipoDisponibles = tipos.Select(t => t.Nombre).OrderBy(x => x).ToList();
                vm.TiposLicenciaDisponibles = licencias.Select(l => l.Nombre).OrderBy(x => x).ToList();

                var codigosEnArchivo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var numerosArchivo = new List<string>();
                for (int r = 2; r <= lastRow; r++)
                {
                    var numero = GetStr(ws.Row(r), "Numero de Activo");
                    if (!string.IsNullOrWhiteSpace(numero))
                        numerosArchivo.Add(numero.Trim());
                }

                var existentesBd = await _activoRepo.ObtenerNumerosExistentesAsync(numerosArchivo);
                var existentesSet = new HashSet<string>(existentesBd, StringComparer.OrdinalIgnoreCase);

                var filasDetectadas = new List<FilaImportacionActivosDto>();

                for (int r = 2; r <= lastRow; r++)
                {
                    var row = ws.Row(r);

                    var fila = new FilaImportacionActivosDto
                    {
                        NumeroFila = r,
                        Seleccionado = true,
                        NumeroActivo = Normalize(GetStr(row, "Numero de Activo")),
                        NombreMaquina = Normalize(GetStr(row, "Nombre de maquina")),
                        TipoEquipo = Normalize(GetStr(row, "Tipo de equipo")),
                        Marca = headerMap.ContainsKey("Marca") ? NormalizeNullable(GetStr(row, "Marca")) : null,
                        Modelo = headerMap.ContainsKey("Model") ? NormalizeNullable(GetStr(row, "Model")) : null,
                        SerieServicio = headerMap.ContainsKey("Serie/Servitag") ? NormalizeNullable(GetStr(row, "Serie/Servitag")) : null,
                        DireccionMAC = headerMap.ContainsKey("MAC") ? NormalizeNullable(GetStr(row, "MAC")) : null,
                        SistemaOperativo = headerMap.ContainsKey("Sistema Operativo") ? NormalizeNullable(GetStr(row, "Sistema Operativo")) : null,
                        TipoLicencia = headerMap.ContainsKey("Tipo de licencia") ? NormalizeNullable(GetStr(row, "Tipo de licencia")) : null,
                        ClaveLicencia = headerMap.ContainsKey("Licencia") ? NormalizeNullable(GetStr(row, "Licencia")) : null
                    };

                    var errores = ValidarFilaEditable(
                        fila,
                        codigosEnArchivo,
                        existentesSet,
                        vm.TiposEquipoDisponibles,
                        vm.TiposLicenciaDisponibles
                    );

                    if (errores.Any())
                    {
                        foreach (var error in errores)
                        {
                            vm.Errores.Add(new FilaValidacionDto
                            {
                                NumeroFila = r,
                                Mensaje = error
                            });
                        }
                    }

                    filasDetectadas.Add(fila);
                }

                vm.TotalFilas = filasDetectadas.Count;
                vm.FilasConError = vm.Errores.Select(e => e.NumeroFila).Where(n => n > 0).Distinct().Count();
                vm.FilasValidas = vm.TotalFilas - vm.FilasConError;
                vm.FilasDetectadas = filasDetectadas;

                hist.TotalFilas = vm.TotalFilas;
                hist.FilasValidas = vm.FilasValidas;
                hist.FilasConError = vm.FilasConError;
                hist.Estado = "Validado";
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

        public async Task<(bool Ok, string Mensaje, ValidacionImportacionDto Resultado)> ConfirmarImportacionInventarioEditadoAsync(
            int historialId,
            List<FilaImportacionActivosDto> filas,
            string? usuarioId)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null) throw new InvalidOperationException("Historial no encontrado.");

            hist.UsuarioEjecutorId ??= usuarioId;

            var tipos = await _catRepo.ObtenerTiposAsync();
            var estados = await _catRepo.ObtenerEstadosAsync();
            var licencias = await _catRepo.ObtenerLicenciasAsync();

            var resultado = new ValidacionImportacionDto
            {
                HistorialId = hist.Id,
                NombreArchivo = hist.NombreArchivo,
                FilasDetectadas = filas ?? new List<FilaImportacionActivosDto>(),
                TiposEquipoDisponibles = tipos.Select(t => t.Nombre).OrderBy(x => x).ToList(),
                TiposLicenciaDisponibles = licencias.Select(l => l.Nombre).OrderBy(x => x).ToList()
            };

            var filasSeleccionadas = (filas ?? new List<FilaImportacionActivosDto>())
                .Where(f => f.Seleccionado)
                .ToList();

            resultado.TotalFilas = filas?.Count ?? 0;

            if (!filasSeleccionadas.Any())
            {
                resultado.FilasConError = 1;
                resultado.Errores.Add(new FilaValidacionDto
                {
                    NumeroFila = 0,
                    Mensaje = "Debe seleccionar al menos un equipo para registrar."
                });

                hist.Estado = "Validado";
                hist.DetalleError = "No se seleccionaron equipos para registrar.";
                await _histRepo.GuardarAsync();

                return (false, "Debe seleccionar al menos un equipo para registrar.", resultado);
            }

            var numerosSeleccionados = filasSeleccionadas
                .Select(f => Normalize(f.NumeroActivo))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var existentesBd = await _activoRepo.ObtenerNumerosExistentesAsync(numerosSeleccionados);
            var existentesSet = new HashSet<string>(existentesBd, StringComparer.OrdinalIgnoreCase);
            var codigosSeleccionados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var fila in filasSeleccionadas)
            {
                fila.NumeroActivo = Normalize(fila.NumeroActivo);
                fila.NombreMaquina = Normalize(fila.NombreMaquina);
                fila.TipoEquipo = Normalize(fila.TipoEquipo);
                fila.Marca = NormalizeNullable(fila.Marca);
                fila.Modelo = NormalizeNullable(fila.Modelo);
                fila.SerieServicio = NormalizeNullable(fila.SerieServicio);
                fila.DireccionMAC = NormalizeNullable(fila.DireccionMAC);
                fila.SistemaOperativo = NormalizeNullable(fila.SistemaOperativo);
                fila.TipoLicencia = NormalizeNullable(fila.TipoLicencia);
                fila.ClaveLicencia = NormalizeNullable(fila.ClaveLicencia);

                var errores = ValidarFilaEditable(
                    fila,
                    codigosSeleccionados,
                    existentesSet,
                    resultado.TiposEquipoDisponibles,
                    resultado.TiposLicenciaDisponibles
                );

                if (errores.Any())
                {
                    foreach (var error in errores)
                    {
                        resultado.Errores.Add(new FilaValidacionDto
                        {
                            NumeroFila = fila.NumeroFila,
                            Mensaje = error
                        });
                    }
                }
            }

            resultado.FilasConError = resultado.Errores.Select(e => e.NumeroFila).Where(n => n > 0).Distinct().Count();
            resultado.FilasValidas = filasSeleccionadas.Count - resultado.FilasConError;

            if (resultado.Errores.Any())
            {
                hist.Estado = "Validado";
                hist.DetalleError = $"Existen {resultado.FilasConError} filas con ajustes pendientes antes de registrar.";
                hist.TotalFilas = resultado.TotalFilas;
                hist.FilasValidas = resultado.FilasValidas;
                hist.FilasConError = resultado.FilasConError;
                await _histRepo.GuardarAsync();

                return (false, "Hay filas con errores. Corrígelas antes de registrar.", resultado);
            }

            var idEstadoOperativo = estados.FirstOrDefault(e => e.Nombre == "Operativo")?.IdEstadoActivo ?? 1;
            var nuevos = new List<ActivoInventario>();

            foreach (var fila in filasSeleccionadas)
            {
                var tipo = tipos.First(t => t.Nombre.Equals(fila.TipoEquipo, StringComparison.OrdinalIgnoreCase));

                int? idTipoLic = null;
                if (!string.IsNullOrWhiteSpace(fila.TipoLicencia))
                {
                    idTipoLic = licencias
                        .FirstOrDefault(l => l.Nombre.Equals(fila.TipoLicencia, StringComparison.OrdinalIgnoreCase))
                        ?.IdTipoLicencia;
                }

                nuevos.Add(new ActivoInventario
                {
                    NumeroActivo = fila.NumeroActivo,
                    NombreMaquina = fila.NombreMaquina,
                    IdTipoActivo = tipo.IdTipoActivo,
                    IdEstadoActivo = idEstadoOperativo,
                    Marca = fila.Marca,
                    Modelo = fila.Modelo,
                    SerieServicio = fila.SerieServicio,
                    DireccionMAC = fila.DireccionMAC,
                    SistemaOperativo = fila.SistemaOperativo,
                    IdTipoLicencia = idTipoLic,
                    ClaveLicencia = fila.ClaveLicencia,
                    FechaCreacion = DateTime.UtcNow
                });
            }

            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                await _activoRepo.AgregarRangoAsync(nuevos);
                await _activoRepo.GuardarAsync();

                hist.Estado = "Importado";
                hist.TotalFilas = resultado.TotalFilas;
                hist.FilasValidas = filasSeleccionadas.Count;
                hist.FilasConError = 0;
                hist.DetalleError = $"Insertados: {nuevos.Count}. Omitidos por deselección: {resultado.TotalFilas - filasSeleccionadas.Count}.";
                await _histRepo.GuardarAsync();

                await tx.CommitAsync();
                return (true, $"Se registraron {nuevos.Count} equipos detectados en el inventario.", resultado);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                hist.Estado = "Fallido";
                hist.DetalleError = ex.Message;
                await _histRepo.GuardarAsync();

                return (false, ex.Message, resultado);
            }
        }

        public async Task<(bool Ok, string Mensaje)> DeshabilitarArchivoAsync(int historialId, string? usuarioId)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null)
                return (false, "No se encontró el archivo solicitado.");

            hist.Estado = "Deshabilitado";
            hist.DetalleError = string.IsNullOrWhiteSpace(hist.DetalleError)
                ? $"Archivo deshabilitado manualmente por el usuario {usuarioId ?? "desconocido"}."
                : $"{hist.DetalleError} | Archivo deshabilitado manualmente por el usuario {usuarioId ?? "desconocido"}.";

            hist.UsuarioEjecutorId = usuarioId;
            hist.Fecha = DateTime.UtcNow;

            await _histRepo.GuardarAsync();

            return (true, "El archivo fue deshabilitado correctamente.");
        }

        public async Task<(bool Ok, string Mensaje)> ReprocesarArchivoAsync(int historialId, string webRootPath, string? usuarioId)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null)
                return (false, "No se encontró el archivo solicitado.");

            if (string.IsNullOrWhiteSpace(hist.RutaArchivo))
                return (false, "El historial no tiene una ruta de archivo válida.");

            var rutaFisica = Path.Combine(
                webRootPath,
                hist.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (!File.Exists(rutaFisica))
                return (false, "El archivo físico no existe para reprocesarlo.");

            try
            {
                hist.Estado = "Cargado";
                hist.DetalleError = null;
                hist.TotalFilas = 0;
                hist.FilasValidas = 0;
                hist.FilasConError = 0;
                hist.UsuarioEjecutorId = usuarioId;
                hist.Fecha = DateTime.UtcNow;

                await _histRepo.ActualizarAsync(hist);
                await _histRepo.GuardarAsync();

                // Si quieres luego, aquí se puede ramificar por hist.Modulo
                var resultado = await ValidarInventarioDesdeExcelAsync(historialId, webRootPath);

                hist = await _histRepo.ObtenerPorIdAsync(historialId);

                if (hist == null)
                    return (false, "No se pudo recuperar el historial después del reproceso.");

                if (string.Equals(hist.Estado, "Fallido", StringComparison.OrdinalIgnoreCase))
                {
                    return (false, $"El archivo fue reprocesado, pero continúa con fallos: {hist.DetalleError ?? "Revise la validación."}");
                }

                if (resultado.Errores.Any())
                {
                    return (true, $"El archivo fue reprocesado. Se detectaron {resultado.FilasConError} fila(s) con error y {resultado.FilasValidas} válida(s).");
                }

                return (true, "El archivo fue reprocesado y validado correctamente.");
            }
            catch (Exception ex)
            {
                hist.Estado = "Fallido";
                hist.DetalleError = ex.Message;
                hist.UsuarioEjecutorId = usuarioId;
                hist.Fecha = DateTime.UtcNow;

                await _histRepo.ActualizarAsync(hist);
                await _histRepo.GuardarAsync();

                return (false, $"No se pudo reprocesar el archivo: {ex.Message}");
            }
        }

        public async Task<ValidacionImportacionTelefonoDto> ValidarTelefonosDesdeExcelAsync(int historialId, string webRootPath)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null) throw new InvalidOperationException("Historial no encontrado.");

            var vm = new ValidacionImportacionTelefonoDto
            {
                HistorialId = hist.Id,
                NombreArchivo = hist.NombreArchivo
            };

            var rutaFisica = Path.Combine(
                webRootPath,
                hist.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            var requeridas = new[] { "Nombre Colaborador" };

            try
            {
                using var wb = new XLWorkbook(rutaFisica);
                var ws = wb.Worksheets.First();

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

                var faltantes = requeridas.Where(r => !headerMap.ContainsKey(r)).ToList();
                if (faltantes.Any())
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = "Faltan columnas requeridas: " + string.Join(", ", faltantes);
                    await _histRepo.GuardarAsync();

                    vm.Errores.Add(new FilaValidacionTelefonoDto
                    {
                        NumeroFila = 0,
                        Mensaje = hist.DetalleError
                    });

                    return vm;
                }

                string GetStr(IXLRow row, string col)
                {
                    if (!headerMap.TryGetValue(col, out var idx)) return "";
                    return row.Cell(idx).GetString().Trim();
                }

                var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

                if (lastRow < 2)
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = "El archivo no contiene filas de datos.";
                    await _histRepo.GuardarAsync();

                    vm.Errores.Add(new FilaValidacionTelefonoDto
                    {
                        NumeroFila = 0,
                        Mensaje = hist.DetalleError
                    });

                    return vm;
                }

                var filasDetectadas = new List<FilaImportacionTelefonoDto>();
                var imeisEnArchivo = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                for (int r = 2; r <= lastRow; r++)
                {
                    var row = ws.Row(r);

                    var fila = new FilaImportacionTelefonoDto
                    {
                        NumeroFila = r,
                        Seleccionado = true,
                        NombreColaborador = Normalize(GetStr(row, "Nombre Colaborador")),
                        Departamento = headerMap.ContainsKey("Departamento") ? NormalizeNullable(GetStr(row, "Departamento")) : null,
                        Operador = headerMap.ContainsKey("Operador") ? NormalizeNullable(GetStr(row, "Operador")) : null,
                        NumeroCelular = headerMap.ContainsKey("Numero Celular") ? NormalizeNullable(GetStr(row, "Numero Celular")) : null,
                        CorreoSistemasAnaliticos = headerMap.ContainsKey("Correo Sistemas Analiticos") ? NormalizeNullable(GetStr(row, "Correo Sistemas Analiticos")) : null,
                        Modelo = headerMap.ContainsKey("Modelo") ? NormalizeNullable(GetStr(row, "Modelo")) : null,
                        IMEI = headerMap.ContainsKey("IMEI") ? NormalizeNullable(GetStr(row, "IMEI")) : null,
                        Cargador = headerMap.ContainsKey("Cargador") && ParseBoolean(GetStr(row, "Cargador")),
                        Auriculares = headerMap.ContainsKey("Auriculares") && ParseBoolean(GetStr(row, "Auriculares"))
                    };

                    var errores = ValidarFilaTelefonoEditable(fila, imeisEnArchivo);

                    if (errores.Any())
                    {
                        foreach (var error in errores)
                        {
                            vm.Errores.Add(new FilaValidacionTelefonoDto
                            {
                                NumeroFila = r,
                                Mensaje = error
                            });
                        }
                    }

                    filasDetectadas.Add(fila);
                }

                vm.TotalFilas = filasDetectadas.Count;
                vm.FilasConError = vm.Errores.Select(e => e.NumeroFila).Where(n => n > 0).Distinct().Count();
                vm.FilasValidas = vm.TotalFilas - vm.FilasConError;
                vm.FilasDetectadas = filasDetectadas;

                hist.TotalFilas = vm.TotalFilas;
                hist.FilasValidas = vm.FilasValidas;
                hist.FilasConError = vm.FilasConError;
                hist.Estado = "Validado";
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

                vm.Errores.Add(new FilaValidacionTelefonoDto
                {
                    NumeroFila = 0,
                    Mensaje = ex.Message
                });

                return vm;
            }
        }

        public async Task<(bool Ok, string Mensaje, ValidacionImportacionTelefonoDto Resultado)> ConfirmarImportacionTelefonosAsync(
            int historialId,
            List<FilaImportacionTelefonoDto> filas,
            string? usuarioId)
        {
            var hist = await _histRepo.ObtenerPorIdAsync(historialId);
            if (hist == null) throw new InvalidOperationException("Historial no encontrado.");

            hist.UsuarioEjecutorId ??= usuarioId;

            var resultado = new ValidacionImportacionTelefonoDto
            {
                HistorialId = hist.Id,
                NombreArchivo = hist.NombreArchivo,
                FilasDetectadas = filas ?? new List<FilaImportacionTelefonoDto>()
            };

            var filasSeleccionadas = (filas ?? new List<FilaImportacionTelefonoDto>())
                .Where(f => f.Seleccionado)
                .ToList();

            resultado.TotalFilas = filas?.Count ?? 0;

            if (!filasSeleccionadas.Any())
            {
                resultado.FilasConError = 1;
                resultado.Errores.Add(new FilaValidacionTelefonoDto
                {
                    NumeroFila = 0,
                    Mensaje = "Debe seleccionar al menos un teléfono para registrar."
                });

                hist.Estado = "Validado";
                hist.DetalleError = "No se seleccionaron teléfonos para registrar.";
                await _histRepo.GuardarAsync();

                return (false, "Debe seleccionar al menos un teléfono para registrar.", resultado);
            }

            var imeisSeleccionados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var fila in filasSeleccionadas)
            {
                fila.NombreColaborador = Normalize(fila.NombreColaborador);
                fila.Departamento = NormalizeNullable(fila.Departamento);
                fila.Operador = NormalizeNullable(fila.Operador);
                fila.NumeroCelular = NormalizeNullable(fila.NumeroCelular);
                fila.CorreoSistemasAnaliticos = NormalizeNullable(fila.CorreoSistemasAnaliticos);
                fila.Modelo = NormalizeNullable(fila.Modelo);
                fila.IMEI = NormalizeNullable(fila.IMEI);

                var errores = ValidarFilaTelefonoEditable(fila, imeisSeleccionados);

                if (errores.Any())
                {
                    foreach (var error in errores)
                    {
                        resultado.Errores.Add(new FilaValidacionTelefonoDto
                        {
                            NumeroFila = fila.NumeroFila,
                            Mensaje = error
                        });
                    }
                }
            }

            resultado.FilasConError = resultado.Errores.Select(e => e.NumeroFila).Where(n => n > 0).Distinct().Count();
            resultado.FilasValidas = filasSeleccionadas.Count - resultado.FilasConError;

            if (resultado.Errores.Any())
            {
                hist.Estado = "Validado";
                hist.DetalleError = $"Existen {resultado.FilasConError} filas con ajustes pendientes antes de registrar.";
                hist.TotalFilas = resultado.TotalFilas;
                hist.FilasValidas = resultado.FilasValidas;
                hist.FilasConError = resultado.FilasConError;
                await _histRepo.GuardarAsync();

                return (false, "Hay filas con errores. Corrígelas antes de registrar.", resultado);
            }

            int insertados = 0;

            foreach (var fila in filasSeleccionadas)
            {
                var dto = new ActivoTelefonoCreateDto
                {
                    NombreColaborador = fila.NombreColaborador,
                    Departamento = fila.Departamento,
                    Operador = fila.Operador,
                    NumeroCelular = fila.NumeroCelular,
                    CorreoSistemasAnaliticos = fila.CorreoSistemasAnaliticos,
                    Modelo = fila.Modelo,
                    IMEI = fila.IMEI,
                    Cargador = fila.Cargador,
                    Auriculares = fila.Auriculares
                };

                var (ok, error) = await _telefonoService.CrearAsync(dto);

                if (!ok)
                {
                    hist.Estado = "Fallido";
                    hist.DetalleError = error ?? "Error al registrar teléfonos.";
                    await _histRepo.GuardarAsync();

                    resultado.Errores.Add(new FilaValidacionTelefonoDto
                    {
                        NumeroFila = fila.NumeroFila,
                        Mensaje = error ?? "Error al registrar teléfono."
                    });

                    return (false, error ?? "No se pudo registrar uno de los teléfonos.", resultado);
                }

                insertados++;
            }

            hist.Estado = "Importado";
            hist.TotalFilas = resultado.TotalFilas;
            hist.FilasValidas = insertados;
            hist.FilasConError = 0;
            hist.DetalleError = $"Insertados: {insertados}. Omitidos por deselección: {resultado.TotalFilas - insertados}.";
            await _histRepo.GuardarAsync();

            return (true, $"Se registraron {insertados} teléfonos en el inventario.", resultado);
        }

        private static List<string> ValidarFilaEditable(
            FilaImportacionActivosDto fila,
            HashSet<string> codigosYaVistos,
            HashSet<string> existentesSet,
            List<string> tiposDisponibles,
            List<string> licenciasDisponibles)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(fila.NumeroActivo))
                errores.Add("Numero de Activo es requerido.");

            if (string.IsNullOrWhiteSpace(fila.NombreMaquina))
                errores.Add("Nombre de maquina es requerido.");

            if (string.IsNullOrWhiteSpace(fila.TipoEquipo))
                errores.Add("Tipo de equipo es requerido.");
            else if (!tiposDisponibles.Contains(fila.TipoEquipo, StringComparer.OrdinalIgnoreCase))
                errores.Add($"Tipo de equipo inválido: '{fila.TipoEquipo}'.");

            if (!string.IsNullOrWhiteSpace(fila.NumeroActivo) && !codigosYaVistos.Add(fila.NumeroActivo))
                errores.Add($"Numero de Activo duplicado en la detección: '{fila.NumeroActivo}'.");

            if (!string.IsNullOrWhiteSpace(fila.NumeroActivo) && existentesSet.Contains(fila.NumeroActivo))
                errores.Add($"Ya existe en BD un activo con NumeroActivo '{fila.NumeroActivo}'.");

            if (!string.IsNullOrWhiteSpace(fila.TipoLicencia) &&
                !licenciasDisponibles.Contains(fila.TipoLicencia, StringComparer.OrdinalIgnoreCase))
                errores.Add($"Tipo de licencia inválido: '{fila.TipoLicencia}'.");

            if (!string.IsNullOrWhiteSpace(fila.NumeroActivo) && !IsValidNumeroActivo(fila.NumeroActivo))
                errores.Add("Numero de Activo tiene formato inválido (solo letras/números y - _ .; 3 a 40 chars).");

            if (!string.IsNullOrWhiteSpace(fila.NombreMaquina) && fila.NombreMaquina.Length > 80)
                errores.Add("Nombre de maquina excede el máximo permitido (80 caracteres).");

            if (!string.IsNullOrWhiteSpace(fila.DireccionMAC) && !IsValidMac(fila.DireccionMAC))
                errores.Add("MAC tiene formato inválido.");

            if (!string.IsNullOrWhiteSpace(fila.SistemaOperativo) && fila.SistemaOperativo.Length > 80)
                errores.Add("Sistema Operativo excede el máximo permitido (80 caracteres).");

            if (!string.IsNullOrWhiteSpace(fila.ClaveLicencia) && string.IsNullOrWhiteSpace(fila.TipoLicencia))
                errores.Add("Si se indica Licencia, también debe indicarse Tipo de licencia.");

            return errores;
        }

        private static List<string> ValidarFilaTelefonoEditable(
            FilaImportacionTelefonoDto fila,
            HashSet<string> imeisYaVistos)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(fila.NombreColaborador))
                errores.Add("Nombre Colaborador es requerido.");

            if (!string.IsNullOrWhiteSpace(fila.IMEI))
            {
                if (!imeisYaVistos.Add(fila.IMEI))
                    errores.Add($"IMEI duplicado en el archivo: '{fila.IMEI}'.");
            }

            if (!string.IsNullOrWhiteSpace(fila.CorreoSistemasAnaliticos) &&
                !fila.CorreoSistemasAnaliticos.Contains("@"))
            {
                errores.Add("Correo Sistemas Analiticos tiene formato inválido.");
            }

            return errores;
        }

        private static string Normalize(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return string.Join(" ", value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        private static string? NormalizeNullable(string? value)
        {
            var normalized = Normalize(value);
            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }

        private static bool ParseBoolean(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var v = value.Trim().ToLowerInvariant();

            return v == "true"
                || v == "1"
                || v == "si"
                || v == "sí"
                || v == "yes"
                || v == "x";
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