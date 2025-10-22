using ApartmentManagementSystem.EF.Repositories.Impls;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using Microsoft.Extensions.DependencyInjection;


namespace ApartmentManagementSystem.EF
{
    public static class RepositoriesServiceRegister
    {
        public static void RegisterRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IApartmentBuildingRepository, ApartmentBuildingRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
