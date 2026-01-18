using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class Request : AuditEntity<Guid>
    {
        [MaxLength(255)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        [MaxLength(50)]
        public string RequestType{ get; set; }
        [MaxLength(25)]
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