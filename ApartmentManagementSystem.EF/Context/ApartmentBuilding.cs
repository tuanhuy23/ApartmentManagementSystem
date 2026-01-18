using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class ApartmentBuilding : AuditEntity<Guid>, ISoftDelete
    {
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string Address { get; set; }
        [MaxLength(1000)]
        public string Description { get;set; }
        [MaxLength(255)]
        public string ContactEmail { get; set; }
        [MaxLength(80)]
        public string ContactPhone { get; set; }
        [MaxLength(25)]
        public string Status { get; set; }
        [MaxLength(25)]
        public string CurrencyUnit { get; set; }
        [MaxLength(255)]
        public string ApartmentBuildingImgUrl { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
        public ICollection<FeeType>? FeeTypes { get; set; }
        public ICollection<Apartment>? Apartments { get; set; }
        public ICollection<Resident>? Residents { get; set; }
        public ICollection<Announcement>? Announcements { get; set; }
        public ICollection<Request>? Requests { get; set; }
        public BillingCycleSetting BillingCycleSetting { get; set; }
        [MaxLength(255)]
        public string? OwnerUserName { get; set; }
        public bool IsDeleted { get ; set ; }
    }
}
