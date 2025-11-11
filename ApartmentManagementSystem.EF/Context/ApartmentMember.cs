using ApartmentManagementSystem.EF.Context.Base;


namespace ApartmentManagementSystem.EF.Context
{
    public class ApartmentMember : AuditEntity<Guid>
    {
        public Guid ApartmentId { get; set; }
        public Apartment Apartment { get; set; }
        public Guid ResidentId { get; set; }
        public Resident Resident { get; set; }
        public string MemberType { get; set; }
    }
    public static class MemberType
    {
        public const string Member = "MEMBER";
        public const string Owner = "OWNER";
    }
}
