
using ApartmentManagementSystem.DbContext;
using ApartmentManagementSystem.Register;
using ApartmentManagementSystem.EF;

namespace ApartmentManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.Get<AppSettings>();

            // Add services to the container.s
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.RegisterService();
            builder.Services.RegisterAuthenticationService();
            builder.Services.RegisterDbContextApartmentManagementService(AppSettings.ConnectionStrings.DataApartment);
            builder.Services.RegisterRepository();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Apartment Management System API",
                    Version = "v1",
                    Description = "API documentation for Apartment Management System (JWT Secured)"
                });
                var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer token only (no 'Bearer' prefix)",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition("Bearer", securityScheme);

                var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                };

                options.AddSecurityRequirement(securityRequirement);
                options.SupportNonNullableReferenceTypes();
            });

            var app = builder.Build();
            
            // Seed data
            var authenticationCtx = app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            bool dbExist = authenticationCtx.Database.EnsureCreated();
            var testUserPw = AppSettings.SeedPwd;
            SeedData.SeedData.Initialize(app.Services.CreateScope().ServiceProvider, testUserPw).GetAwaiter().GetResult();

            app.UseCors(
               options => options.WithOrigins("http://localhost").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()
           );
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Apartment Management System API v1");
                    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
