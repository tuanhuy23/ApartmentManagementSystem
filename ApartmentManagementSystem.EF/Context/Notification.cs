using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Notification : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public Guid RelatedEntityID { get; set; }
        public string RelatedEntityType { get; set; }
        public bool IsRead { get; set; }
    }
    public static class RelatedEntityType
    {
        public const string FeeNotice = "FEENOTICE";
        public const string Request = "REQUEST";
    }
}