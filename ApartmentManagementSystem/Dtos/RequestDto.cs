namespace ApartmentManagementSystem.Dtos
{
    public class RequestDto
    {
        public Guid Id { get; set;}
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string? UserId { get; set; } 
        public IEnumerable<FileAttachmentDto> Files { get; set; }
        public IEnumerable<FeedbackDto> Feedbacks { get; set; }
        public Guid ApartmentBuildingId { get; set; }
    }
    public class FileAttachmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Src { get; set; }
        public string FileType { get; set; }
    }
    public class FeedbackDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public Guid RequestId { get;set;}
        public IEnumerable<FileAttachmentDto> Files { get; set; }
    }
}