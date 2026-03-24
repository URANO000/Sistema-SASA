using System.ComponentModel.DataAnnotations;
using DataAccess.Modelos.DTOs.Inventario;

namespace SASA.ViewModels.Inventario
{
    public class CrearActivoViewModel
    {
        [Required(ErrorMessage = "El código del activo es requerido.")]
        [StringLength(40, ErrorMessage = "El código del activo no puede superar los 40 caracteres.")]
        [MaxLength(40)]
        public string NumeroActivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del activo es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre del activo no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string NombreMaquina { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un tipo.")]
        public int IdTipoActivo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un estado.")]
        public int IdEstadoActivo { get; set; }

        [StringLength(50, ErrorMessage = "La marca no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string? Marca { get; set; }

        [StringLength(50, ErrorMessage = "El modelo no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string? Modelo { get; set; }

        [StringLength(50, ErrorMessage = "La serie o service tag no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string? SerieServicio { get; set; }

        [StringLength(17, ErrorMessage = "La dirección MAC no puede superar los 17 caracteres.")]
        [MaxLength(17)]
        public string? DireccionMAC { get; set; }

        [StringLength(50, ErrorMessage = "El sistema operativo no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string? SistemaOperativo { get; set; }

        public int? IdTipoLicencia { get; set; }

        [StringLength(100, ErrorMessage = "La clave de licencia no puede superar los 100 caracteres.")]
        [MaxLength(100)]
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