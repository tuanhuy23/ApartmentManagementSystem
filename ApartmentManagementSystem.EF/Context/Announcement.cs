using System.Collections;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Announcement : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public bool IsAll { get; set; }
        public IEnumerable<string>? ApartmentIds { get; set; }
        public DateTime PublishDate { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
        public ICollection<UserReadStatus> UserReadStatuses { get; set; }
    }
}