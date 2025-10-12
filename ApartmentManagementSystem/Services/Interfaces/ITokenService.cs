using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;

namespace ApartmentManagementSystem.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponseDto> LoginAsync(LoginRequestDto request);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task<ChangePasswordResponseDto> ChangePassword(ChangePasswordRequestDto request);
        Task<UpdatePasswordInFristTimeLoginResponseDto> UpdatePasswordInFristTimeLogin(UpdatePasswordInFristTimeLoginRequestDto request);
    }
}
