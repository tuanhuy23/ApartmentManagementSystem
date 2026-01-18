using System.ComponentModel.DataAnnotations;
using ApartmentManagementSystem.EF.Context.Base;

namespace ApartmentManagementSystem.EF.Context
{
    public class FileAttachment : EntityBase<Guid>
    {
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(512)]
        public string? Description { get; set; }
        [MaxLength(255)]
        public string Src { get; set; }
        [MaxLength(25)]
        public string FileType { get; set; }
        public Guid? ApartmentBuildingId { get; set; }
        public ApartmentBuilding? ApartmentBuilding { get; set; }
        public Announcement? Announcement { get; set; }
        public Guid? AnnouncementId { get; set; }
        public Request? Request { get; set; }
        public Guid? RequestId { get; set; }
        public RequestHistory? Feedback { get; set; }
        public Guid? FeedbackId { get; set; }
    }
    public static class FileType
    {
        public const string Doc = "Doc";
        public const string Media = "MEDIA";
    }
}
