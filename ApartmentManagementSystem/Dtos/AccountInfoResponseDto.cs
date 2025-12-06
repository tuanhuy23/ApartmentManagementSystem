namespace ApartmentManagementSystem.Dtos
{
    public class AccountInfoResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string ApartmentBuildingId{ get; set; }
        public string ApartmentId {get; set;}
        public string IsActive { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}