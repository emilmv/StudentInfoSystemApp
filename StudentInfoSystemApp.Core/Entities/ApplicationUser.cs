using Microsoft.AspNetCore.Identity;

namespace StudentInfoSystemApp.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
