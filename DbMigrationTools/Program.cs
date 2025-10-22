using DbMigrationTools.ContextFactory;
using Microsoft.EntityFrameworkCore;

namespace DbMigrationTools
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var viewCtx = new ApartmentManagementDbContextFactory().CreateDbContext(null);
            viewCtx.Database.Migrate();
        }
    }
}
