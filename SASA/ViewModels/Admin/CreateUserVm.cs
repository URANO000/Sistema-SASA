using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Admin
{
    public class CreateUserVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public string? PrimerNombre { get; set; }
        public string? PrimerApellido { get; set; }

        [Required, DataType(DataType.Password)]
        public string TempPassword { get; set; } = "";

        public bool Estado { get; set; } = true;
    }
}
