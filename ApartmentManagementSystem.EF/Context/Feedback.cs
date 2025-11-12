using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Feedback : AuditEntity<Guid>
    {
        public string Description { get; set; }
        public int Rate { get; set; }
        public Request Request { get; set; }
        public Guid RequestId { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
        public Guid ApartmentBuildingId { get; set; }
    }
}