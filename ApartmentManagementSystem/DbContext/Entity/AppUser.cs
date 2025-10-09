using Microsoft.AspNetCore.Identity;

namespace ApartmentManagementSystem.DbContext.Entity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Position { get; set; }
    }
}
