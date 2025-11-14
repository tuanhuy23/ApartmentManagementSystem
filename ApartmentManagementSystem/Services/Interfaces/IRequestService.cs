using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IRequestService
    {
        public IEnumerable<RequestDto> GetRequests();
        public RequestDto GetRequest();
        public Task CreateOrUpdateRequest(RequestDto request);
        public Task DeleteRequest (Guid requestId);
        public Task CreateOrUpdateFeedback(FeedbackDto request);
        public Task DeleteFeedback (Guid feedbackId);
    }
}