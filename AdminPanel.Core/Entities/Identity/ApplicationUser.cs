using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Core.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
