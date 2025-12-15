using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Auth
{
    public class ResetPasswordViewModel
    {
        public string? Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "La confirmación es requerida.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
