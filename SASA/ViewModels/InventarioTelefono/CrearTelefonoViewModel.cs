using System.ComponentModel.DataAnnotations;
using DataAccess.Modelos.DTOs.InventarioTelefono;

namespace SASA.ViewModels.InventarioTelefono
{
    public class CrearTelefonoViewModel
    {
        public int IdActivoTelefono { get; set; }

        [Required(ErrorMessage = "El nombre del colaborador es requerido.")]
        public string NombreColaborador { get; set; } = string.Empty;

        public string? Departamento { get; set; }

        public string? Operador { get; set; }

        public string? NumeroCelular { get; set; }

        public string? CorreoSistemasAnaliticos { get; set; }

        public string? Modelo { get; set; }

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