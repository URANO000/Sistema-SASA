using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.Inventario;
using DataAccess.Repositorios.Inventario;

namespace BusinessLogic.Servicios.Inventario
{
    public class InventarioService : IInventarioService
    {
        private readonly IActivoInventarioRepository _repo;
        private readonly IActivoInventarioTiqueteRepository _asociacionRepo;
        private readonly IMantenimientoActivoRepository _mantenimientoRepo;
        private readonly ICatalogosInventarioRepository _catRepo;

        public InventarioService(
            IActivoInventarioRepository repo,
            IActivoInventarioTiqueteRepository asociacionRepo,
            IMantenimientoActivoRepository mantenimientoRepo,
            ICatalogosInventarioRepository catRepo)
        {
            _repo = repo;
            _asociacionRepo = asociacionRepo;
            _mantenimientoRepo = mantenimientoRepo;
            _catRepo = catRepo;
        }

        public async Task<PagedResult<ActivoTelefonoInventarioListItemDto>> ListarPaginadoAsync(ActivoInventarioFiltroDto filtros)
        {
            var page = filtros.Page < 1 ? 1 : filtros.Page;
            var pageSize = filtros.PageSize < 5 ? 5 : filtros.PageSize;
            if (pageSize > 50) pageSize = 50;

            var total = await _repo.ContarAsync(filtros.Texto, filtros.IdEstadoActivo, filtros.IdTipoActivo);

            var skip = (page - 1) * pageSize;

            var data = await _repo.ListarPaginadoAsync(
                filtros.Texto,
                filtros.IdEstadoActivo,
                filtros.IdTipoActivo,
                skip,
                pageSize,
                filtros.SortBy,
                filtros.SortDir
            );

            var items = data.Select(a => new ActivoTelefonoInventarioListItemDto
            {
                IdActivo = a.IdActivo,
                NumeroActivo = a.NumeroActivo,
                NombreMaquina = a.NombreMaquina,
                SerieServicio = a.SerieServicio,
                IdTipoActivo = a.IdTipoActivo,
                IdEstadoActivo = a.IdEstadoActivo,
                TipoActivoNombre = a.TipoActivo?.Nombre,
                EstadoActivoNombre = a.EstadoActivo?.Nombre,
                FechaCreacion = a.FechaCreacion
            }).ToList();

            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            return new PagedResult<ActivoTelefonoInventarioListItemDto>
            {
                Items = items,
                TotalRecords = total,
                TotalPages = totalPages,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<IReadOnlyList<ActivoTelefonoInventarioListItemDto>> ObtenerActivosParaAsociacionAsync()
        {
            var activos = await _repo.ListarAsync(null, null, null);

            return activos
                .Select(a => new ActivoTelefonoInventarioListItemDto
                {
                    IdActivo = a.IdActivo,
                    NumeroActivo = a.NumeroActivo,
                    NombreMaquina = a.NombreMaquina,
                    SerieServicio = a.SerieServicio,
                    IdTipoActivo = a.IdTipoActivo,
                    IdEstadoActivo = a.IdEstadoActivo,
                    TipoActivoNombre = a.TipoActivo?.Nombre,
                    EstadoActivoNombre = a.EstadoActivo?.Nombre,
                    FechaCreacion = a.FechaCreacion
                })
                .ToList();
        }

        public async Task<ActivoInventarioDetailDto?> ObtenerDetalleAsync(int id)
        {
            var a = await _repo.ObtenerDetalleAsync(id);
            if (a == null) return null;

            return new ActivoInventarioDetailDto
            {
                IdActivo = a.IdActivo,
                NumeroActivo = a.NumeroActivo,
                NombreMaquina = a.NombreMaquina,
                Marca = a.Marca,
                Modelo = a.Modelo,
                SerieServicio = a.SerieServicio,
                DireccionMAC = a.DireccionMAC,
                SistemaOperativo = a.SistemaOperativo,
                ClaveLicencia = a.ClaveLicencia,
                IdTipoActivo = a.IdTipoActivo,
                IdEstadoActivo = a.IdEstadoActivo,
                IdTipoLicencia = a.IdTipoLicencia,
                TipoActivoNombre = a.TipoActivo?.Nombre,
                EstadoActivoNombre = a.EstadoActivo?.Nombre,
                FechaCreacion = a.FechaCreacion,
                FechaActualizacion = a.FechaActualizacion
            };
        }

        public async Task<(bool ok, string? error)> CrearAsync(ActivoInventarioCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NumeroActivo))
                return (false, "El código del activo es requerido.");

            if (string.IsNullOrWhiteSpace(dto.NombreMaquina))
                return (false, "El nombre del activo es requerido.");

            var numero = dto.NumeroActivo.Trim();

            if (await _repo.ExisteNumeroActivoAsync(numero))
                return (false, "Ya existe un activo con este código.");

            var entity = new ActivoInventario
            {
                NumeroActivo = numero,
                NombreMaquina = dto.NombreMaquina.Trim(),
                IdTipoActivo = dto.IdTipoActivo,
                IdEstadoActivo = dto.IdEstadoActivo,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                SerieServicio = dto.SerieServicio,
                DireccionMAC = dto.DireccionMAC,
                SistemaOperativo = dto.SistemaOperativo,
                IdTipoLicencia = dto.IdTipoLicencia,
                ClaveLicencia = dto.ClaveLicencia,
                FechaCreacion = DateTime.UtcNow
            };

            await _repo.CrearAsync(entity);
            await _repo.GuardarAsync();

            return (true, null);
        }

        public async Task<(bool ok, string? error)> ActualizarAsync(int id, ActivoInventarioEditDto dto)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);
            if (entity == null)
                return (false, "Activo no encontrado.");

            if (!string.Equals(entity.NumeroActivo, dto.NumeroActivo, StringComparison.OrdinalIgnoreCase))
                return (false, "No se permite modificar el Número de Activo.");

            var estados = await _catRepo.ObtenerEstadosAsync();

            var estadoMantenimiento = estados.FirstOrDefault(x =>
                string.Equals(x.Nombre, "Mantenimiento", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(x.Nombre, "En Mantenimiento", StringComparison.OrdinalIgnoreCase));

            var mantenimientoEnProceso = await _mantenimientoRepo.ExisteMantenimientoEnProcesoAsync(id);

            if (mantenimientoEnProceso && estadoMantenimiento != null && dto.IdEstadoActivo != estadoMantenimiento.IdEstadoActivo)
            {
                return (false, "No se puede cambiar el estado del activo mientras tenga un mantenimiento en proceso.");
            }

            entity.IdEstadoActivo = dto.IdEstadoActivo;
            entity.IdTipoActivo = dto.IdTipoActivo;
            entity.NombreMaquina = dto.NombreMaquina;
            entity.Marca = dto.Marca;
            entity.Modelo = dto.Modelo;
            entity.SerieServicio = dto.SerieServicio;
            entity.DireccionMAC = dto.DireccionMAC;
            entity.SistemaOperativo = dto.SistemaOperativo;
            entity.IdTipoLicencia = dto.IdTipoLicencia;
            entity.ClaveLicencia = dto.ClaveLicencia;
            entity.FechaActualizacion = DateTime.UtcNow;

            await _repo.GuardarAsync();
            return (true, null);
        }

        public async Task<(bool ok, string? error)> AsociarActivoConTiqueteAsync(ActivoTiqueteAsociacionDto dto)
        {
            var activo = await _repo.ObtenerPorIdAsync(dto.IdActivo);
            if (activo == null)
                return (false, "El activo seleccionado no existe.");

            var yaExiste = await _asociacionRepo.ExisteAsociacionAsync(dto.IdActivo, dto.IdTiquete);
            if (yaExiste)
                return (false, "Este activo ya está asociado a ese tiquete.");

            var entity = new ActivoInventarioTiquete
            {
                IdActivo = dto.IdActivo,
                IdTiquete = dto.IdTiquete,
                FechaAsociacion = DateTime.UtcNow
            };

            await _asociacionRepo.CrearAsync(entity);
            await _asociacionRepo.GuardarAsync();

            return (true, null);
        }

        public async Task<IReadOnlyList<MantenimientoActivoListItemDto>> ObtenerHistorialMantenimientoAsync(int? idActivo = null)
        {
            var data = idActivo.HasValue
                ? await _mantenimientoRepo.ListarPorActivoAsync(idActivo.Value)
                : await _mantenimientoRepo.ListarAsync();

            return data.Select(x => new MantenimientoActivoListItemDto
            {
                IdMantenimiento = x.IdMantenimiento,
                IdActivo = x.IdActivo,
                NumeroActivo = x.Activo?.NumeroActivo ?? string.Empty,
                NombreMaquina = x.Activo?.NombreMaquina,
                FechaMantenimiento = x.FechaMantenimiento,
                TipoMantenimiento = x.TipoMantenimiento,
                Estado = x.Estado,
                Descripcion = x.Descripcion
            }).ToList();
        }

        public async Task<MantenimientoActivoListItemDto?> ObtenerMantenimientoPorIdAsync(int id)
        {
            var x = await _mantenimientoRepo.ObtenerPorIdAsync(id);
            if (x == null) return null;

            return new MantenimientoActivoListItemDto
            {
                IdMantenimiento = x.IdMantenimiento,
                IdActivo = x.IdActivo,
                NumeroActivo = x.Activo?.NumeroActivo ?? string.Empty,
                NombreMaquina = x.Activo?.NombreMaquina,
                FechaMantenimiento = x.FechaMantenimiento,
                TipoMantenimiento = x.TipoMantenimiento,
                Estado = x.Estado,
                Descripcion = x.Descripcion
            };
        }

        public async Task<IReadOnlyList<ActivoTelefonoInventarioListItemDto>> ObtenerActivosReporteGeneralAsync()
        {
            var data = await _repo.ListarAsync(null, null, null);

            return data.Select(a => new ActivoTelefonoInventarioListItemDto
            {
                IdActivo = a.IdActivo,
                NumeroActivo = a.NumeroActivo,
                NombreMaquina = a.NombreMaquina,
                SerieServicio = a.SerieServicio,
                IdTipoActivo = a.IdTipoActivo,
                IdEstadoActivo = a.IdEstadoActivo,
                TipoActivoNombre = a.TipoActivo?.Nombre,
                EstadoActivoNombre = a.EstadoActivo?.Nombre,
                FechaCreacion = a.FechaCreacion
            }).ToList();
        }

        public async Task<Dictionary<string, int>> ObtenerResumenPorEstadoAsync()
        {
            var data = await _repo.ListarAsync(null, null, null);

            return data
                .GroupBy(x => x.EstadoActivo?.Nombre ?? "Sin estado")
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<(bool ok, string? error)> RegistrarMantenimientoAsync(CrearMantenimientoActivoDto dto)
        {
            var activo = await _repo.ObtenerPorIdAsync(dto.IdActivo);
            if (activo == null)
                return (false, "El activo seleccionado no existe.");

            if (string.IsNullOrWhiteSpace(dto.TipoMantenimiento))
                return (false, "El tipo de mantenimiento es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Estado))
                return (false, "El estado es requerido.");

            var entity = new MantenimientoActivo
            {
                IdActivo = dto.IdActivo,
                FechaMantenimiento = dto.FechaMantenimiento,
                TipoMantenimiento = dto.TipoMantenimiento.Trim(),
                Estado = dto.Estado.Trim(),
                Descripcion = dto.Descripcion,
                FechaCreacion = DateTime.UtcNow
            };

            await _mantenimientoRepo.CrearAsync(entity);
            await _mantenimientoRepo.GuardarAsync();

            await SincronizarEstadoActivoPorMantenimientoAsync(dto.IdActivo, dto.Estado);

            return (true, null);
        }

        public async Task<(bool ok, string? error)> ActualizarMantenimientoAsync(int id, CrearMantenimientoActivoDto dto)
        {
            var entity = await _mantenimientoRepo.ObtenerPorIdAsync(id);
            if (entity == null)
                return (false, "Mantenimiento no encontrado.");

            var activo = await _repo.ObtenerPorIdAsync(dto.IdActivo);
            if (activo == null)
                return (false, "El activo seleccionado no existe.");

            if (string.IsNullOrWhiteSpace(dto.TipoMantenimiento))
                return (false, "El tipo de mantenimiento es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Estado))
                return (false, "El estado es requerido.");

            entity.IdActivo = dto.IdActivo;
            entity.FechaMantenimiento = dto.FechaMantenimiento;
            entity.TipoMantenimiento = dto.TipoMantenimiento.Trim();
            entity.Estado = dto.Estado.Trim();
            entity.Descripcion = dto.Descripcion;

            await _mantenimientoRepo.ActualizarAsync(entity);
            await _mantenimientoRepo.GuardarAsync();

            await SincronizarEstadoActivoPorMantenimientoAsync(dto.IdActivo, dto.Estado, entity.IdMantenimiento);

            return (true, null);
        }

        private async Task SincronizarEstadoActivoPorMantenimientoAsync(int idActivo, string estadoMantenimiento, int? excluirIdMantenimiento = null)
        {
            var activo = await _repo.ObtenerPorIdAsync(idActivo);
            if (activo == null) return;

            var estados = await _catRepo.ObtenerEstadosAsync();

            if (string.Equals(estadoMantenimiento, "En proceso", StringComparison.OrdinalIgnoreCase))
            {
                var estadoMantenimientoDb = estados.FirstOrDefault(x =>
                    string.Equals(x.Nombre, "Mantenimiento", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(x.Nombre, "En Mantenimiento", StringComparison.OrdinalIgnoreCase));

                if (estadoMantenimientoDb != null)
                {
                    activo.IdEstadoActivo = estadoMantenimientoDb.IdEstadoActivo;
                    await _repo.GuardarAsync();
                }

                return;
            }

            if (string.Equals(estadoMantenimiento, "Finalizado", StringComparison.OrdinalIgnoreCase))
            {
                var existeOtroEnProceso = await _mantenimientoRepo.ExisteMantenimientoEnProcesoAsync(idActivo, excluirIdMantenimiento);

                if (!existeOtroEnProceso)
                {
                    var estadoActivoDb = estados.FirstOrDefault(x =>
                        string.Equals(x.Nombre, "Activo", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(x.Nombre, "Operativo", StringComparison.OrdinalIgnoreCase));

                    if (estadoActivoDb != null)
                    {
                        activo.IdEstadoActivo = estadoActivoDb.IdEstadoActivo;
                        await _repo.GuardarAsync();
                    }
                }
            }
        }
    }
}
