using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetUsers(string appartmentBuildingId = "");
        Task<UserDto> GetUser(string userId);
        Task<UserDto> CreateOrUpdateUser(CreateOrUpdateUserRequestDto request);
        Task<DeleteUserResponseDto> DeleteUsers(IEnumerable<string> userIds);
    }
}
