using System.ComponentModel.DataAnnotations;
using DataAccess.Modelos.DTOs.InventarioTelefono;

namespace SASA.ViewModels.InventarioTelefono
{
    public class CrearTelefonoViewModel
    {
        public int IdActivoTelefono { get; set; }

        [Required(ErrorMessage = "El nombre del colaborador es requerido.")]
        [StringLength(80, ErrorMessage = "El nombre del colaborador no puede superar los 80 caracteres.")]
        [MaxLength(80)]
        public string NombreColaborador { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El departamento no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string? Departamento { get; set; }

        [StringLength(30, ErrorMessage = "El operador no puede superar los 30 caracteres.")]
        [MaxLength(30)]
        public string? Operador { get; set; }

        [StringLength(20, ErrorMessage = "El número celular no puede superar los 20 caracteres.")]
        [MaxLength(20)]
        public string? NumeroCelular { get; set; }

        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        public string? CorreoSistemasAnaliticos { get; set; }

        [StringLength(50, ErrorMessage = "El modelo no puede superar los 50 caracteres.")]
        [MaxLength(50)]
        public string? Modelo { get; set; }

        [StringLength(20, ErrorMessage = "El IMEI no puede superar los 20 caracteres.")]
        [MaxLength(20)]
        public string? IMEI { get; set; }

        public bool Cargador { get; set; }

        public bool Auriculares { get; set; }

        public ActivoTelefonoCreateDto ToCreateDto() => new()
        {
            NombreColaborador = NombreColaborador,
            Departamento = Departamento,
            Operador = Operador,
            NumeroCelular = NumeroCelular,
            CorreoSistemasAnaliticos = CorreoSistemasAnaliticos,
            Modelo = Modelo,
            IMEI = IMEI,
            Cargador = Cargador,
            Auriculares = Auriculares
        };

        public ActivoTelefonoEditDto ToEditDto() => new()
        {
            NombreColaborador = NombreColaborador,
            Departamento = Departamento,
            Operador = Operador,
            NumeroCelular = NumeroCelular,
            CorreoSistemasAnaliticos = CorreoSistemasAnaliticos,
            Modelo = Modelo,
            IMEI = IMEI,
            Cargador = Cargador,
            Auriculares = Auriculares
        };
    }
}