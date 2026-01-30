namespace SASA.ViewModels.Admin
{
    public class UserListItemVm
    {
        public string Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool Estado { get; set; }
        public bool EmailConfirmed { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}
