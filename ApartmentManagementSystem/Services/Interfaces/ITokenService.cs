using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponseDto> LoginAsync(LoginRequestDto request);
        Task<TokenResponseDto> RefreshTokenAsync(string token);
        Task LogoutAsync(string refreshToken);
    }
}
