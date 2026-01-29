using Microsoft.AspNetCore.Identity;

namespace DataAccess.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public bool Estado { get; set; } = true;
    }
}