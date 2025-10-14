namespace ApartmentManagementSystem.Dtos
{
    public class DeleteRoleResponse
    {
        public List<string> RoleIdsDeleteSuccess { get; set; }
        public List<string> RoleIdsDeleteError { get; set; }
    }
}
