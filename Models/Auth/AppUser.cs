using Microsoft.AspNetCore.Identity;

namespace Indigo.Models.Auth
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
