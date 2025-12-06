using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class ApartmentBuilding : AuditEntity<Guid>, ISoftDelete
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get;set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Status { get; set; }
        public string CurrencyUnit { get; set; }
        public string ApartmentBuildingImgUrl { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
        public ICollection<FeeType>? FeeTypes { get; set; }
        public ICollection<Apartment>? Apartments { get; set; }
        public ICollection<Resident>? Residents { get; set; }
        public ICollection<Announcement>? Announcements { get; set; }
        public ICollection<Request>? Requests { get; set; }
        public BillingCycleSetting BillingCycleSetting { get; set; }
        public string? OwnerUserName { get; set; }
        public bool IsDeleted { get ; set ; }
    }
}
