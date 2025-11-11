using ApartmentManagementSystem.EF.Context.Base;


namespace ApartmentManagementSystem.EF.Context
{
    public class Resident : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public string Name { get; set; }
        public DateTime? BrithDay { get; set; }
        public string? IdentityNumber { get; set; }
        public string? UserId { get; set; }
        public ICollection<ApartmentMember> ApartmentMembers { get; set; }
    }
}
