namespace ApartmentManagementSystem.Dtos
{
    public class CreateOrUpdateUserRequestDto
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string RoleId { get; set; }
        public string Possition { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string AppartmentId { get; set; }
    }
}