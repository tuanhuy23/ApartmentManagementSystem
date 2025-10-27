using Microsoft.AspNetCore.Identity;

namespace ApartmentManagementSystem.DbContext.Entity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public string? AppartmentBuildingId { get; set; }
    }
}
