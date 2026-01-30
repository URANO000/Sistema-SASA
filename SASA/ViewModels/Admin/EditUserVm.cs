using System.ComponentModel.DataAnnotations;

namespace SASA.ViewModels.Admin
{
    public class EditUserVm
    {
        [Required]
        public string Id { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        public bool Estado { get; set; }
    }
}
