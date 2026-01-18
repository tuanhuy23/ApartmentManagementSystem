using System.Collections;
using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Announcement : AuditEntity<Guid>
    {
        public Guid ApartmentBuildingId { get; set; }
        public ApartmentBuilding ApartmentBuilding { get; set; }
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Body { get; set; }
        [MaxLength(25)]
        public string Status { get; set; }
        public bool IsAll { get; set; }
        public IEnumerable<string>? ApartmentIds { get; set; }
        public DateTime PublishDate { get; set; }
        public ICollection<FileAttachment> Files { get; set; }
    }
}