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
        public DbSet<FeeRateConfig> FeeRateConfigs { get; set; }
        public DbSet<FeeTier> FeeTiers { get; set; }
        public DbSet<BillingCycleSetting> BillingCycleSettings { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ParkingRegistration> ParkingRegistrations { get; set; }
        public DbSet<FeeNotice> FeeNotices { get; set; }
        public DbSet<FeeDetail> FeeDetails { get; set; }
        public DbSet<UtilityReading> UtilityReadings{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
    {
            #region ApartmentBuilding
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Images)
              .WithOne(ci => ci.ApartmentBuilding)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.ApartmentBuildingId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApartmentBuilding>().HasMany(c => c.FeeTypes)
             .WithOne(ci => ci.ApartmentBuilding)
             .HasPrincipalKey(ci => ci.Id)
             .HasForeignKey(c => c.ApartmentBuildingId)
             .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApartmentBuilding>().HasOne(c => c.BillingCycleSetting)
              .WithOne(ci => ci.ApartmentBuilding)
              .HasForeignKey<BillingCycleSetting>(p => p.ApartmentBuildingId)
              .IsRequired(true);

            builder.Entity<ApartmentBuilding>().HasMany(c => c.Apartments)
            .WithOne(ci => ci.ApartmentBuilding)
            .HasPrincipalKey(ci => ci.Id)
            .HasForeignKey(c => c.ApartmentBuildingId)
            .OnDelete(DeleteBehavior.Cascade);   
            #endregion

            #region  FeeType
            builder.Entity<FeeType>().HasMany(c => c.FeeRateConfigs)
              .WithOne(ci => ci.FeeType)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.FeeTypeId)
              .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FeeType>().HasMany(c => c.UtilityReadings)
              .WithOne(ci => ci.FeeType)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.FeeTypeId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FeeType>().HasMany(c => c.FeeDetails)
              .WithOne(ci => ci.FeeType)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.FeeTypeId)
              .OnDelete(DeleteBehavior.Restrict);
            #endregion
              
            #region  FeeRateConfig
            builder.Entity<FeeRateConfig>().HasMany(c => c.FeeTiers)
              .WithOne(ci => ci.FeeRateConfig)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.FeeRateConfigId)
              .OnDelete(DeleteBehavior.Cascade);
            #endregion
              
            #region Apartment
            builder.Entity<Apartment>().HasMany(c => c.ParkingRegistrations)
              .WithOne(ci => ci.Apartment)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.ApartmentId)
              .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Apartment>().HasMany(c => c.UtilityReadings)
              .WithOne(ci => ci.Apartment)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.ApartmentId)
              .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Apartment>().HasMany(c => c.FeeNotices)
              .WithOne(ci => ci.Apartment)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.ApartmentId)
              .OnDelete(DeleteBehavior.Cascade);
            #endregion
            
            #region FeeNotice
            builder.Entity<FeeNotice>().HasMany(c => c.FeeDetails)
              .WithOne(ci => ci.FeeNotice)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.FeeNoticeId)
              .OnDelete(DeleteBehavior.Cascade);
            
            #endregion
        }
    }
}
