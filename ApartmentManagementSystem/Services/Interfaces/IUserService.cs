using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<Pagination<UserDto>> GetUsers(RequestQueryBaseDto<string> request);
        Task<UserDto> GetUser(string userId);
        Task<UserDto> CreateOrUpdateUser(CreateOrUpdateUserRequestDto request);
        Task<DeleteUserResponseDto> DeleteUsers(IEnumerable<string> userIds);
        Task<IEnumerable<UserDto>> GetAllUsers(string apartmentBuidlingId);
    }
}
