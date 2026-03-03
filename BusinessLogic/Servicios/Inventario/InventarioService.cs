using DataAccess.Modelos.DTOs.Inventario;
using DataAccess.Modelos.DTOs.Wrappers;
using DataAccess.Modelos.Entidades.Inventario;
using DataAccess.Repositorios.Inventario;

namespace BusinessLogic.Servicios.Inventario
{
    public class InventarioService : IInventarioService
    {
        private readonly IActivoInventarioRepository _repo;

        public InventarioService(IActivoInventarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<ActivoInventarioListItemDto>> ListarPaginadoAsync(ActivoInventarioFiltroDto filtros)
        {
            var page = filtros.Page < 1 ? 1 : filtros.Page;
            var pageSize = filtros.PageSize < 5 ? 5 : filtros.PageSize;
            if (pageSize > 50) pageSize = 50;

            var total = await _repo.ContarAsync(filtros.Texto, filtros.IdEstadoActivo, filtros.IdTipoActivo);

            var skip = (page - 1) * pageSize;

            // ✅ ahora pasa sortBy/sortDir
            var data = await _repo.ListarPaginadoAsync(
                filtros.Texto,
                filtros.IdEstadoActivo,
                filtros.IdTipoActivo,
                skip,
                pageSize,
                filtros.SortBy,
                filtros.SortDir
            );

            var items = data.Select(a => new ActivoInventarioListItemDto
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

            return new PagedResult<ActivoInventarioListItemDto>
            {
                Items = items,
                TotalRecords = total,
                TotalPages = totalPages,
                PageNumber = page,
                PageSize = pageSize
            };
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
            if (entity == null) return (false, "Activo no encontrado.");

            // Regla Sprint 1: no se permite cambiar el código
            if (!string.Equals(entity.NumeroActivo, dto.NumeroActivo, StringComparison.OrdinalIgnoreCase))
                return (false, "No se permite modificar el Número de Activo.");

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
    }
}