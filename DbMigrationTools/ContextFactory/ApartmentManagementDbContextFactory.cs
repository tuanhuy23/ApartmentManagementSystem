using ApartmentManagementSystem.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DbMigrationTools.ContextFactory
{
    internal class ApartmentManagementDbContextFactory : IDesignTimeDbContextFactory<ApartmentManagementDbContext>
    {
        public ApartmentManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApartmentManagementDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ApartmentManagement;Username=postgres;Password=aod@1234;Trust Server Certificate=true", b => b.MigrationsAssembly("DbMigrationTools"));
            return new ApartmentManagementDbContext(optionsBuilder.Options);
        }
    }
}
