namespace ApartmentManagementSystem.Dtos
{
    public class ResidentDto
    {
        public Guid? Id { get; set; }
        public string UserId { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public string Name { get; set; }
        public DateTime? BrithDay { get; set; }
        public string? IdentityNumber { get; set; }
        public string MemberType { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName{get;set;}
        public string Password { get;set; }
        public Guid ApartmentId { get; set; }
        public bool IsOwner { get; set; }
    }
}
