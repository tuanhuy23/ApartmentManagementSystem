using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Request : AuditEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequestType{ get; set; }
        public string Status { get; set; }
        public string? CurrentHandlerId { get; set; } 
        public ICollection<FileAttachment> Files { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public Guid ResidentId { get; set; }
        public int? Rate { get; set; }
        public ICollection<RequestHistory> RequestHistories { get; set; }
    }
}