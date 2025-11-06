using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.EF.Repositories.Impls;
using ApartmentManagementSystem.EF.Repositories.Impls.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace ApartmentManagementSystem.EF
{
    public static class RepositoriesServiceRegister
    {
        public static void RegisterRepository(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IApartmentBuildingRepository, ApartmentBuildingRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IApartmentRepository, ApartmentRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IBillingCycleSettingRepository, BillingCycleSettingRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IFeeTypeRepository, FeeTypeRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IParkingRegistrationRepository, ParkingRegistrationRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IUtilityReadingRepository, UtilityReadingRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>)).AddScoped<IFeeNoticeRepository, FeeNoticeRepository>();
                 
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        public static void RegisterDbContextApartmentManagementService(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApartmentManagementDbContext>((optionBuilder) =>
            {
                optionBuilder.UseSqlServer(connectionString);
            });
            services.AddScoped<Func<ApartmentManagementDbContext>>((provider) => () => provider.GetService<ApartmentManagementDbContext>());
            services.AddScoped<DbFactory>();

        }
    }
}
