using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Auth
{
    public class SetPasswordViewModel
    {
        [Required]
        public string Token { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string NewPassword { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = "";
    }
}
