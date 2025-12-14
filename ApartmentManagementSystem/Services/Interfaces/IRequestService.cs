using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IRequestService
    {
        public Task<Pagination<RequestDto>> GetRequests(RequestQueryBaseDto<Guid> request);
        public RequestDto GetRequest(Guid requestId);
        public Task CreateOrUpdateRequest(RequestDto request);
        public Task DeleteRequest (List<string> requestId);
        public Task UpdateStatusAndAssignRequest(UpdateStatusAndAssignRequestDto request);
        public Task CreateOrUpdateRequestAction(RequestHistoryDto request);
        public Task RattingRequest(RattingRequestDto request);
       public Task<IEnumerable<UserDto>> GetUserHandlers(string apartmentBuidlingId);
    }
}