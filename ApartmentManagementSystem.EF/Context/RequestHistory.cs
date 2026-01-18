using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class RequestHistory : AuditEntity<Guid>
    {
        [MaxLength(1000)]
        public string? Note { get; set; }
        [MaxLength(255)]
        public string ActionType { get; set; }
        [MaxLength(25)]
        public string? OldStatus { get; set; }
        [MaxLength(25)]
        public string? NewStatus { get; set; }
        public string? NewUserAssignId { get; set; }
        public Request Request { get; set; }
        public Guid RequestId { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
        public Guid ApartmentBuildingId { get; set; }
    }
}