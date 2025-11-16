namespace ApartmentManagementSystem.Dtos
{
    public class AccountInfoResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string ApartmentBuildingId{ get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}