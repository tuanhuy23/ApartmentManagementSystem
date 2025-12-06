using ApartmentManagementSystem.Identity;

namespace ApartmentManagementSystem.Dtos
{
    public class RoleDto
    {
        public string RoleId { get;set; }
        public string RoleName { get;set; }
        public List<PermissionInfo> Permissions { get; set; }
        public string AppartmentBuildingId { get; set; }
    }
}
