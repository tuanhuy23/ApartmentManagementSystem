using ApartmentManagementSystem.DbContext.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.DbContext
{
    public class AuthenticationDbContext : IdentityDbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
}
