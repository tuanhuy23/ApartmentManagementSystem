using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.Identity;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Pagination<RoleDto>> GetRoles(RequestQueryBaseDto<string> request);
        Task<RoleDto> GetRole(string roleId);
        Task<RoleDto> CreateOrUpdateRole(RoleDto request);
        Task<DeleteRoleResponse> DeleteRoles(IEnumerable<string> roleIds);
        Task<string> GetRoleIdByRoleName(string roleName);
         Task<IEnumerable<PermissionInfo>> GetPermissionInfos();

    }
}
