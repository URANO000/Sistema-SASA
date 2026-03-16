using DataAccess.Modelos.DTOs.InventarioTelefono;
using DataAccess.Modelos.Entidades.InventarioTelefono;
using DataAccess.Repositorios.InventarioTelefono;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Servicios.InventarioTelefono
{
    public class ActivoTelefonoService : IActivoTelefonoService
    {
        private readonly IActivoTelefonoRepository _repo;

        public ActivoTelefonoService(IActivoTelefonoRepository repo)
        {
            _repo = repo;
        }

        public async Task<(List<ActivoTelefonoListItemDto> Items, int TotalRecords)>
            ListarPaginadoAsync(ActivoTelefonoFiltroDto filtros)
        {
            if (filtros.Page < 1) filtros.Page = 1;
            if (filtros.PageSize < 5) filtros.PageSize = 5;
            if (filtros.PageSize > 50) filtros.PageSize = 50;

            int skip = (filtros.Page - 1) * filtros.PageSize;

            var total = await _repo.ContarAsync(filtros.Texto);

            var data = await _repo.ListarPaginadoAsync(
                filtros.Texto,
                skip,
                filtros.PageSize,
                filtros.SortBy,
                filtros.SortDir
            );

            var items = data.Select(x => new ActivoTelefonoListItemDto
            {
                IdActivoTelefono = x.IdActivoTelefono,
                NombreColaborador = x.NombreColaborador,
                Departamento = x.Departamento,
                Operador = x.Operador,
                NumeroCelular = x.NumeroCelular,
                Modelo = x.Modelo
            }).ToList();

            return (items, total);
        }

        public async Task<ActivoTelefonoDetailDto?> ObtenerDetalleAsync(int id)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);

            if (entity == null)
                return null;

            return new ActivoTelefonoDetailDto
            {
                IdActivoTelefono = entity.IdActivoTelefono,
                NombreColaborador = entity.NombreColaborador,
                Departamento = entity.Departamento,
                Operador = entity.Operador,
                NumeroCelular = entity.NumeroCelular,
                CorreoSistemasAnaliticos = entity.CorreoSistemasAnaliticos,
                Modelo = entity.Modelo,
                IMEI = entity.IMEI,
                Cargador = entity.Cargador,
                Auriculares = entity.Auriculares
            };
        }

        public async Task<(bool Ok, string? Error)> CrearAsync(ActivoTelefonoCreateDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.IMEI))
            {
                var existeImei = await _repo.ExisteImeiAsync(dto.IMEI);
                if (existeImei)
                    return (false, "El IMEI ingresado ya está registrado en otro teléfono.");
            }

            var entity = new ActivoTelefono
            {
                NombreColaborador = dto.NombreColaborador,
                Departamento = dto.Departamento,
                Operador = dto.Operador,
                NumeroCelular = dto.NumeroCelular,
                CorreoSistemasAnaliticos = dto.CorreoSistemasAnaliticos,
                Modelo = dto.Modelo,
                IMEI = dto.IMEI,
                Cargador = dto.Cargador,
                Auriculares = dto.Auriculares,
                FechaCreacion = DateTime.Now
            };

            try
            {
                await _repo.CrearAsync(entity);
                await _repo.GuardarAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo guardar el teléfono. Verifique que el IMEI no esté duplicado.");
            }
            catch (Exception)
            {
                return (false, "Ocurrió un error inesperado al registrar el teléfono.");
            }
        }

        public async Task<(bool Ok, string? Error)> ActualizarAsync(int id, ActivoTelefonoEditDto dto)
        {
            var entity = await _repo.ObtenerPorIdAsync(id);

            if (entity == null)
                return (false, "Teléfono no encontrado.");

            if (!string.IsNullOrWhiteSpace(dto.IMEI))
            {
                var existeImei = await _repo.ExisteImeiAsync(dto.IMEI, id);
                if (existeImei)
                    return (false, "El IMEI ingresado ya está registrado en otro teléfono.");
            }

            entity.NombreColaborador = dto.NombreColaborador;
            entity.Departamento = dto.Departamento;
            entity.Operador = dto.Operador;
            entity.NumeroCelular = dto.NumeroCelular;
            entity.CorreoSistemasAnaliticos = dto.CorreoSistemasAnaliticos;
            entity.Modelo = dto.Modelo;
            entity.IMEI = dto.IMEI;
            entity.Cargador = dto.Cargador;
            entity.Auriculares = dto.Auriculares;
            entity.FechaActualizacion = DateTime.Now;

            try
            {
                await _repo.ActualizarAsync(entity);
                await _repo.GuardarAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "No se pudo actualizar el teléfono. Verifique que el IMEI no esté duplicado.");
            }
            catch (Exception)
            {
                return (false, "Ocurrió un error inesperado al actualizar el teléfono.");
            }
        }
    }
}