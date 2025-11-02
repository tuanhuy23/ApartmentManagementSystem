using ApartmentManagementSystem.Consts;
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
            await EnsureRole(serviceProvider, RoleDefaulConsts.SupperAdmin, adminID);
            await EnsureRole(serviceProvider, RoleDefaulConsts.Management);
            await EnsureRole(serviceProvider, RoleDefaulConsts.Resident);
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
                    UserName = email,
                    IsActive = true,
                    AppartmentBuildingId = "root"
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
                                                                      string role, string uid = "")
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
            switch (role)
            {
                case RoleDefaulConsts.SupperAdmin:
                    await roleManager.SeedClaimsForSuperAdmin();
                    break;
                case RoleDefaulConsts.Management:
                    await roleManager.SeedClaimForManagement();
                    break;
                case RoleDefaulConsts.Resident:
                    await roleManager.SeedClaimForResident();
                    break;
                default:
                    await roleManager.SeedClaimsForSuperAdmin();
                    break;

            }
            if (!string.IsNullOrEmpty(uid))
            {
                var userManager = serviceProvider.GetService<UserManager<AppUser>>();
                if (userManager == null) throw new Exception("userManager is null");
                var user = await userManager.FindByIdAsync(uid);
                if (user == null)
                {
                    throw new Exception("The testUserPw password was probably not strong enough!");
                }
                IR = await userManager.AddToRoleAsync(user, role);
            }
            return IR;
        }

        private async static Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync(RoleDefaulConsts.SupperAdmin);
            if (adminRole == null) return;
            await roleManager.AddPermissionClaim(adminRole, "ApartmentBuildingPermissions");
            await roleManager.AddPermissionClaim(adminRole, "UserPermissions");
        }
        private async static Task SeedClaimForManagement(this RoleManager<IdentityRole> roleManager)
        {
            var managementRole = await roleManager.FindByNameAsync(RoleDefaulConsts.Management);
            if (managementRole == null) return;
            await roleManager.AddPermissionClaim(managementRole, "RolePermissions");
            await roleManager.AddPermissionClaim(managementRole, "UserPermissions");
            await roleManager.AddPermissionClaim(managementRole, "ApartmentPermissions");
            await roleManager.AddPermissionClaim(managementRole, "FeeConfigurationPermissions");
            await roleManager.AddPermissionClaim(managementRole, "FeeNoticePermissions");
            await roleManager.AddPermissionClaim(managementRole, "NotificationPermissions");
            await roleManager.AddPermissionClaim(managementRole, "RequestPermissions");
        }
        private async static Task SeedClaimForResident(this RoleManager<IdentityRole> roleManager)
        {
            var residentRole = await roleManager.FindByNameAsync(RoleDefaulConsts.Resident);
            if (residentRole == null) return;
            await roleManager.AddPermissionClaim(residentRole, "RequestPermissions");
            await roleManager.AddPermissionClaim(residentRole, "FeeNoticePermissions", false);
            await roleManager.AddPermissionClaim(residentRole, "NotificationPermissions", false);
        }
        private static List<string> GenerateAllPermissionsForModule(string module)
        {
            var result = new List<string>()
            {
                $"Permissions.{module}.Read",
                $"Permissions.{module}.ReadWrite",
            };
            return result;
        }
        private static List<string> GenerateReadPermissionsForModule(string module)
        {
            var result = new List<string>()
            {
                $"Permissions.{module}.Read",
            };
            return result;
        }
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module,  bool getAll = true)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = getAll ? GenerateAllPermissionsForModule(module) : GenerateReadPermissionsForModule(module);
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
