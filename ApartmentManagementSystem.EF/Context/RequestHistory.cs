using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class RequestHistory : AuditEntity<Guid>
    {
        public string? Note { get; set; }
        public string ActionType { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public Request Request { get; set; }
        public Guid RequestId { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
        public Guid ApartmentBuildingId { get; set; }
    }
}