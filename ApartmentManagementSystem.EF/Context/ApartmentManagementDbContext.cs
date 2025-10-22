using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.EF.Context
{
    public class ApartmentManagementDbContext : DbContext
    {
        public ApartmentManagementDbContext(DbContextOptions<ApartmentManagementDbContext> options)
       : base(options)
        {
        }
        public DbSet<ApartmentBuilding> ApartmentBuildings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Images)
              .WithOne(ci => ci.ApartmentBuilding)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.ApartmentBuildingId)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
