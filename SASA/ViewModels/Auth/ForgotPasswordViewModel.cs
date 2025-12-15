using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string? Email { get; set; }
    }
}
