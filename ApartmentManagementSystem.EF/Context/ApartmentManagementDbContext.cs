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
        public DbSet<UtilityReading> UtilityReadings { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<ApartmentResident> ApartmentResidents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<FileAttachment> FileAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
    {
            #region ApartmentBuilding
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Files)
              .WithOne(ci => ci.ApartmentBuilding)
              .HasForeignKey(c => c.ApartmentBuildingId)
              .OnDelete(DeleteBehavior.NoAction);

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
            
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Residents)
            .WithOne(ci => ci.ApartmentBuilding)
            .HasPrincipalKey(ci => ci.Id)
            .HasForeignKey(c => c.ApartmentBuildingId)
            .OnDelete(DeleteBehavior.Cascade);   
            
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Announcements)
            .WithOne(ci => ci.ApartmentBuilding)
            .HasPrincipalKey(ci => ci.Id)
            .HasForeignKey(c => c.ApartmentBuildingId)
            .OnDelete(DeleteBehavior.Cascade);  
            
            builder.Entity<ApartmentBuilding>().HasMany(c => c.Requests)
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

            builder.Entity<FeeType>().HasMany(c => c.QuantityRateConfigs)
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

            builder.Entity<FeeDetail>().HasMany(c => c.FeeDetailTiers)
             .WithOne(ci => ci.FeeDetail)
             .HasPrincipalKey(ci => ci.Id)
             .HasForeignKey(c => c.FeeDetailId)
             .OnDelete(DeleteBehavior.Cascade);

            #endregion
            
            #region Resident 
            
            builder.Entity<ApartmentResident>().HasOne(c => c.Apartment)
             .WithMany(ci => ci.ApartmentResidents)
             .HasForeignKey(c => c.ApartmentId)
             .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<ApartmentResident>().HasOne(c => c.Resident)
             .WithMany(ci => ci.ApartmentResidents)
             .HasForeignKey(c => c.ResidentId)
             .OnDelete(DeleteBehavior.NoAction);
             
            #endregion
            
            #region Notification 
            
            builder.Entity<Notification>().HasIndex(u => u.UserId);
             
            #endregion
            
            #region Announcement 
            
            builder.Entity<Announcement>().HasMany(c => c.Files)
              .WithOne(ci => ci.Announcement)
              .HasForeignKey(c => c.AnnouncementId)
              .OnDelete(DeleteBehavior.NoAction);
              
            builder.Entity<Announcement>().HasMany(c => c.UserReadStatuses)
              .WithOne(ci => ci.Announcement)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.AnnouncementId)
              .OnDelete(DeleteBehavior.Cascade);
              
            builder.Entity<Announcement>(e =>
            {
                e.Property(e => e.ApartmentIds)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries));
            });
            
            builder.Entity<UserReadStatus>().HasIndex(u => new { u.UserId, u.AnnouncementId});
            #endregion
            
            #region Request 
            
            builder.Entity<Request>().HasMany(c => c.Files)
              .WithOne(ci => ci.Request)
              .HasForeignKey(c => c.RequestId)
              .OnDelete(DeleteBehavior.NoAction);
            
            builder.Entity<Request>().HasMany(c => c.RequestHistories)
              .WithOne(ci => ci.Request)
              .HasPrincipalKey(ci => ci.Id)
              .HasForeignKey(c => c.RequestId)
              .OnDelete(DeleteBehavior.NoAction);
              
            builder.Entity<RequestHistory>().HasMany(c => c.Files)
              .WithOne(ci => ci.Feedback)
              .HasForeignKey(c => c.FeedbackId)
              .OnDelete(DeleteBehavior.NoAction);
             
            #endregion
        }
    }
}
