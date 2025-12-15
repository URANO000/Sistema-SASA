using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
