namespace ApartmentManagementSystem.Dtos
{
    public class RequestDto
    {
        public Guid? Id { get; set;}
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string RequestType{ get; set; }
        public string? CurrentHandlerId { get; set; } 
        public IEnumerable<FileAttachmentDto> Files { get; set; }
        public IEnumerable<RequestHistoryDto> RequestHistories { get; set; }
        public Guid ApartmentBuildingId { get; set; }
        public int? Rate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? CreatedDisplayUser { get; set; }
    }
    public class FileAttachmentDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Src { get; set; }
        public string FileType { get; set; }
    }
    public class RequestHistoryDto
    {
        public Guid? Id { get; set; }
        public string? Note { get; set; }
        public Guid RequestId { get;set;}
        public IEnumerable<FileAttachmentDto> Files { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? CreatedDisplayUser { get; set; }
        
    }
    public class UpdateStatusAndAssignRequestDto
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
        public string? CurrentHandlerId { get; set; }
    }
    public class RattingRequestDto
    {
        public Guid Id { get; set; }
        public int Ratting { get; set; }
    }
}