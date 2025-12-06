using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.DbContext;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.EF;
using ApartmentManagementSystem.Filters;
using ApartmentManagementSystem.Identity;
using ApartmentManagementSystem.Services;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ApartmentManagementSystem.Register
{
    public static class ServiceRegister
    {
        public static void RegisterService(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IApartmentBuildingService, ApartmentBuildingService>();
            services.AddScoped<IApartmentService, ApartmentService>();
            services.AddScoped<IParkingRegistrationService, ParkingRegistrationService>();
            services.AddScoped<IBillingCycleSettingService, BillingCycleSettingService>();
            services.AddScoped<IFeeConfigurationService, FeeConfigurationService>();
            services.AddScoped<IFeeService, FeeService>();
            services.AddScoped<IResidentService, ResidentService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped(serviceProvider =>
            {
                var context = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;

                if (context == null)
                {
                    return new UserAudit()
                    {
                        UserId = "superadmin@gmail.com",
                        UserName = "superadmin@gmail.com"
                    };
                }
                var accountInfo = IdentityHelper.GetIdentity(context);
                if (accountInfo != null)
                {
                    return new UserAudit()
                    {
                        UserId = accountInfo.Id,
                        UserName = accountInfo.UserName
                    };
                }
                return new UserAudit()
                {
                    UserId = "superadmin@gmail.com",
                    UserName = "superadmin@gmail.com"
                };
            });
            services.AddScoped<ICloudinaryService, CloudinaryService>(serviceProvider =>
            {
                return new CloudinaryService(AppSettings.Cloudinary);
            });
            services.AddScoped<ApartmentBuildingValidationFilter>();
            services.AddSingleton(new ApartmentBuildingData());
        }
        public static void RegisterAuthenticationService(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });


            //Add Authentication
            services.AddDbContext<AuthenticationDbContext>(options =>
               options.UseNpgsql(AppSettings.ConnectionStrings.Identity));

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddDefaultTokenProviders();

            var key = Encoding.UTF8.GetBytes(AppSettings.JwtSettings.Secret);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                  options.RequireHttpsMetadata = false; // set true in production
                  options.SaveToken = true;
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = AppSettings.JwtSettings.Issuer,
                      ValidAudience = AppSettings.JwtSettings.Audience,
                      IssuerSigningKey = new SymmetricSecurityKey(key),
                      ClockSkew = TimeSpan.Zero
                  };
            });

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }
}
