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
            optionsBuilder.UseSqlServer("Server=.;Database=ApartmentManagement;User Id=sa;Password=aod@123;TrustServerCertificate=True;", b => b.MigrationsAssembly("DbMigrationTools"));
            return new ApartmentManagementDbContext(optionsBuilder.Options);
        }
    }
}
