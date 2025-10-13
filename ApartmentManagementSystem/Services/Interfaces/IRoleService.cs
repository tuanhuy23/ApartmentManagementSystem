using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetRoles();
        Task<RoleDto> GetRole(string roleId);
        Task<RoleDto> CreateOrUpdateRole(RoleDto request);
        Task<RoleDto> CreateOrUpdateRole(RoleDto request);
    }
}
