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
        public DbSet<FeeType> FeeTypes { get; set; }
        public DbSet<FeeRateConfig> FeeRateConfig { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Images)
              .WithOne(ci => ci.ApartmentBuilding)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.ApartmentBuildingId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApartmentBuilding>().HasMany(c => c.FeeTypes)
             .WithOne(ci => ci.ApartmentBuilding)
             .HasPrincipalKey(ci => ci.Id)
             .HasForeignKey(c => c.ApartmentBuildingId)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApartmentBuilding>().HasMany(c => c.FeeRateConfigs)
            .WithOne(ci => ci.ApartmentBuilding)
            .HasPrincipalKey(ci => ci.Id)
            .HasForeignKey(c => c.ApartmentBuildingId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeType>().HasMany(c => c.FeeRateConfigs)
            .WithOne(ci => ci.FeeType)
            .HasPrincipalKey(ci => ci.Id)
            .HasForeignKey(c => c.FeeTypeId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeRateConfig>().HasMany(c => c.FeeTiers)
           .WithOne(ci => ci.FeeRateConfig)
           .HasPrincipalKey(ci => ci.Id)
           .HasForeignKey(c => c.FeeRateConfigId)
           .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
