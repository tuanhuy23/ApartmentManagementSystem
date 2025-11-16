namespace ApartmentManagementSystem.Dtos
{
    public class AnnouncementDto
    {
        public Guid? Id { get;set;}
        public Guid ApartmentBuildingId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Status { get; set; }
        public bool IsAll { get; set; }
        public IEnumerable<Guid>? ApartmentIds { get; set; }
        public DateTime PublishDate { get; set; }
        public ICollection<FileAttachmentDto> Files { get; set; }
    }
}