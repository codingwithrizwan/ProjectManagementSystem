using Microsoft.AspNetCore.Identity;

namespace PMS.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FullName { get; private set; }

        private ApplicationUser() { }

        public ApplicationUser(string userName, string? email = null, string? fullName = null)
        {
            UserName = userName;
            Email = email;
            FullName = fullName;
        }
    }
}
