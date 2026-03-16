using System.ComponentModel.DataAnnotations;
using DataAccess.Modelos.DTOs.Inventario;

namespace SASA.ViewModels.Inventario
{
    public class CrearActivoViewModel
    {
        [Required(ErrorMessage = "El código del activo es requerido.")]
        public string NumeroActivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del activo es requerido.")]
        public string NombreMaquina { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un tipo.")]
        public int IdTipoActivo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public int IdEstadoActivo { get; set; }

        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? SerieServicio { get; set; }
        public string? DireccionMAC { get; set; }
        public string? SistemaOperativo { get; set; }

        public int? IdTipoLicencia { get; set; }
        public string? ClaveLicencia { get; set; }

        public ActivoInventarioCreateDto ToDto() => new()
        {
            NumeroActivo = NumeroActivo,
            NombreMaquina = NombreMaquina,
            IdTipoActivo = IdTipoActivo,
            IdEstadoActivo = IdEstadoActivo,
            Marca = Marca,
            Modelo = Modelo,
            SerieServicio = SerieServicio,
            DireccionMAC = DireccionMAC,
            SistemaOperativo = SistemaOperativo,
            IdTipoLicencia = IdTipoLicencia,
            ClaveLicencia = ClaveLicencia
        };
    }
}