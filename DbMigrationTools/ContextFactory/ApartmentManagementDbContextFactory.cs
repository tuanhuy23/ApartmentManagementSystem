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
            optionsBuilder.UseNpgsql("Host=db.exgflfqhyemhzzasuqmq.supabase.co;Database=postgres;Username=postgres;Password=4HDt2c2st1KepJd3;SSL Mode=Require;Trust Server Certificate=true", b => b.MigrationsAssembly("DbMigrationTools"));
            return new ApartmentManagementDbContext(optionsBuilder.Options);
        }
    }
}
