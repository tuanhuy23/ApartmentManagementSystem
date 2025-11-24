using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IRequestService
    {
        public Pagination<RequestDto> GetRequests(RequestQueryBaseDto<Guid> request);
        public RequestDto GetRequest(Guid requestId);
        public Task CreateOrUpdateRequest(RequestDto request);
        public Task DeleteRequest (Guid requestId);
        public Task CreateOrUpdateFeedback(FeedbackDto request);
        public Task DeleteFeedback (Guid feedbackId);
    }
}