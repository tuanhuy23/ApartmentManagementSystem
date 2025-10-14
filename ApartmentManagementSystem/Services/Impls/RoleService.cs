using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Identity;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApartmentManagementSystem.Services.Impls
{
    class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<RoleDto> CreateOrUpdateRole(RoleDto request)
        {
            if (string.IsNullOrEmpty(request.RoleId))
            {
                await _roleManager.CreateAsync(new IdentityRole(request.RoleName));
                var roleNew = await _roleManager.FindByNameAsync(request.RoleName);
                var allClaims = await _roleManager.GetClaimsAsync(roleNew);
                foreach (var permission in request.Permissions)
                {
                    if (allClaims.Any(a => a.Type == "Permission" && a.Value == permission.Name)) continue;
                    await _roleManager.AddClaimAsync(roleNew, new Claim("Permission", permission.Name));
                }
                request.RoleId = roleNew.Id;
                return request;
            }
            var roleUpdate = await _roleManager.FindByIdAsync(request.RoleId);
            var allClaimsUpdate = await _roleManager.GetClaimsAsync(roleUpdate);

            foreach (var permission in allClaimsUpdate)
            {
                await _roleManager.RemoveClaimAsync(roleUpdate, permission);
            }
            foreach (var permission in request.Permissions)
            {
                await _roleManager.AddClaimAsync(roleUpdate, new Claim("Permission", permission.Name));
            }
            return request;
        }

        public async Task<DeleteRoleResponse> DeleteRoles(IEnumerable<string> roleIds)
        {
            var result = new DeleteRoleResponse()
            {
                RoleIdsDeleteError = new List<string>(),
                RoleIdsDeleteSuccess = new List<string>()
            };
            foreach (var roleId in roleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null) 
                {
                    result.RoleIdsDeleteError.Add(roleId);
                    continue;
                }         
                var lstUserInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                foreach (var userRole in lstUserInRole)
                {
                    await _userManager.RemoveFromRoleAsync(userRole, role.Name);
                }
                var allClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var permission in allClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, permission);
                }
                await _roleManager.DeleteAsync(role);
                result.RoleIdsDeleteSuccess.Add(roleId);
            }
            return result;
        }

        public async Task<RoleDto> GetRole(string roleId)
        {
            RoleDto roleDto = new RoleDto();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                roleDto.RoleId = role.Id;
                roleDto.RoleName = role.Name;
                var allClaims = await _roleManager.GetClaimsAsync(role);
                roleDto.Permissions = new List<PermissionInfo>();
                foreach (var permission in allClaims)
                {
                    roleDto.Permissions.Add(new PermissionInfo()
                    {
                        Name = permission.Value,
                        Selected = true,
                        Type = permission.Type
                    });
                }
            }
            return roleDto;
        }

        public async Task<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            List<RoleDto> roleDtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                if (!RoleDefaulConsts.SupperAdmin.Equals(role.Name))
                {
                    var roleDto = new RoleDto()
                    {
                        RoleName = role.Name,
                        RoleId = role.Id
                    };
                    roleDtos.Add(roleDto);
                }
            }
            return roleDtos;
        }
    }
}
