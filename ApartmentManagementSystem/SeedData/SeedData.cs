using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.DbContext.Entity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ApartmentManagementSystem.SeedData
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            var adminID = await EnsureUser(serviceProvider, testUserPw, "superadmin@gmail.com");
            await EnsureRole(serviceProvider, adminID, RoleDefaulConsts.SupperAdmin);
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string email)
        {
            var userManager = serviceProvider.GetService<UserManager<AppUser>>();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    Email = email,
                    EmailConfirmed = true,
                    DisplayName = email,
                    UserName = email
                };
                await userManager.CreateAsync(user, testUserPw);
            }
            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }
            var userManager = serviceProvider.GetService<UserManager<AppUser>>();
            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }
            IR = await userManager.AddToRoleAsync(user, role);
            await roleManager.SeedClaimsForSuperAdmin();
            return IR;
        }
        private async static Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(RoleDefaulConsts.SupperAdmin);
            await roleManager.AddPermissionClaim(adminRole, "RolePermissions");
            await roleManager.AddPermissionClaim(adminRole, "UserPermissions");
        }
        private static List<string> GeneratePermissionsForModule(string module)
        {
            var result = new List<string>()
            {
                $"Permissions.{module}.Read",
                $"Permissions.{module}.ReadWrite",
                $"Permissions.{module}.ReadWriteAll",
            };
            return result;
        }
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
