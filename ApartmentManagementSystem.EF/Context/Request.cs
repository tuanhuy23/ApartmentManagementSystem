using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Request : AuditEntity<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RequestType{ get; set; }
        public string Status { get; set; }
        public string? UserId { get; set; } 
        public ICollection<FileAttachment> Files { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public Guid ApartmentBuildingId { get; set; }
    }
}