using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Identity;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApartmentManagementSystem.Services.Impls
{
    class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RoleService(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<RoleDto> CreateOrUpdateRole(RoleDto request)
        {
            if (string.IsNullOrEmpty(request.RoleId))
            {
                await _roleManager.CreateAsync(new AppRole()
                {
                    AppartmentBuildingId = request.AppartmentBuildingId,
                    Name = request.RoleName
                });
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

        public async Task<IEnumerable<PermissionInfo>> GetPermissionInfos()
        {
            return PermissionsHelper.GetPermissionInfos(); 
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

        public async Task<string> GetRoleIdByRoleName(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                throw new DomainException(ErrorCodeConsts.RoleNotFound, ErrorCodeConsts.RoleNotFound, System.Net.HttpStatusCode.NotFound);
            return role.Id;
        }

        public async Task<Pagination<RoleDto>> GetRoles(RequestQueryBaseDto<string> request)
        {
            var roles = _roleManager.Roles.Where(r => request.Request.Equals(r.AppartmentBuildingId)).ToList();
            List<RoleDto> roleDtos = new List<RoleDto>();
            foreach (var role in roles)
            {
                if (!RoleDefaulConsts.SupperAdmin.Equals(role.Name))
                {
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var roleDto = new RoleDto()
                    {
                        RoleName = role.Name,
                        RoleId = role.Id,
                        Permissions = GetPermissions(claims)
                    };
                    roleDtos.Add(roleDto);
                }
            }
             var roleQuery = roleDtos.AsQueryable();
            if (request.Filters!= null && request.Filters.Any())
            {
                roleQuery = FilterHelper.ApplyFilters(roleQuery, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                roleQuery = SortHelper.ApplySort(roleQuery, request.Sorts);
            }
            return new Pagination<RoleDto>()
            {
                Items = roleQuery.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = roleQuery.Count()
            };
        }
        private List<PermissionInfo> GetPermissions(IList<Claim> claims)
        {
            return claims.Select(c => PermissionsHelper.GetPermissionInfo(c.Value, c.ValueType)).ToList();
        }
    }
}
