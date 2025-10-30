using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.DbContext;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApartmentManagementSystem.Services.Impls
{
    class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthenticationDbContext _authenticationDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly HttpContext _httpContext = null;
        public TokenService(UserManager<AppUser> userManager, AuthenticationDbContext authenticationDbContext, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _authenticationDbContext = authenticationDbContext;
            _roleManager = roleManager;
            if (httpContextAccessor.HttpContext != null)
            {
                _httpContext = httpContextAccessor.HttpContext;
            }
        }
        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null) throw new DomainException(ErrorCodeConsts.UserNameOrPasswordNotInCorrect, ErrorMessageConsts.UserNameOrPasswordNotInCorrect, System.Net.HttpStatusCode.BadRequest);

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new DomainException(ErrorCodeConsts.UserNameOrPasswordNotInCorrect, ErrorMessageConsts.UserNameOrPasswordNotInCorrect, System.Net.HttpStatusCode.BadRequest);

            var tokenResponse = await GenerateTokenAsync(user);
            var hashToken = CreateHasToken(tokenResponse.RefreshToken);
            string entityRefreshTokenId = Guid.NewGuid().ToString();
            var entityRefreshToken = new RefreshToken
            {
                Id = entityRefreshTokenId,
                TokenHash = hashToken.Hash,
                TokenSalt = hashToken.Salt,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
            };
            _authenticationDbContext.RefreshToken.Add(entityRefreshToken);
            await _authenticationDbContext.SaveChangesAsync();
            
            tokenResponse.RefreshToken = $"{entityRefreshTokenId}.{tokenResponse.RefreshToken}"; 
            return tokenResponse;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var parts = refreshToken.Split('.');
            if (parts.Length != 2)
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, System.Net.HttpStatusCode.BadRequest);
                
            var refreshTokenId = parts[0];
            var refreshTokenValue = parts[1];

            var storedToken = await _authenticationDbContext.RefreshToken.FirstOrDefaultAsync(x => x.Id == refreshTokenId);
            if (storedToken == null) 
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, System.Net.HttpStatusCode.BadRequest);

            if (!VerifyHasToken(refreshTokenValue, storedToken.TokenSalt, storedToken.TokenHash))
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, System.Net.HttpStatusCode.BadRequest);

            storedToken.Revoked = DateTime.UtcNow;
            await _authenticationDbContext.SaveChangesAsync();
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var parts = refreshToken.Split('.');
            if (parts.Length != 2)
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, System.Net.HttpStatusCode.BadRequest);

            var refreshTokenId = parts[0];
            var refreshTokenValue = parts[1];

            var storedToken = await _authenticationDbContext.RefreshToken.FirstOrDefaultAsync(x => x.Id == refreshTokenId);

            if (storedToken == null || storedToken.IsExpired || storedToken.IsRevoked)
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, System.Net.HttpStatusCode.BadRequest);

            if (!VerifyHasToken(refreshTokenValue, storedToken.TokenSalt, storedToken.TokenHash))
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, System.Net.HttpStatusCode.BadRequest);

            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            if (user == null) throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.BadRequest);

            var tokenResponse = await GenerateTokenAsync(user);
            var hashToken = CreateHasToken(tokenResponse.RefreshToken);
            storedToken.TokenHash = hashToken.Hash;
            storedToken.TokenSalt = hashToken.Salt;
            storedToken.ReplacedByTokenHash = hashToken.Hash;
            await _authenticationDbContext.SaveChangesAsync();
            tokenResponse.RefreshToken = $"{storedToken.Id}.{tokenResponse.RefreshToken}";
            return tokenResponse;
        }

        public async Task<ChangePasswordResponseDto> ChangePassword(ChangePasswordRequestDto request)
        {
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            if (accountInfo == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.BadRequest);

            var user = await _userManager.FindByIdAsync(accountInfo.Id);
            if (user == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.BadRequest);

            if (!await _userManager.CheckPasswordAsync(user, request.OldPassword))
                throw new DomainException(ErrorCodeConsts.OldPasswordNotInCorrect, ErrorMessageConsts.OldPasswordNotInCorrect, System.Net.HttpStatusCode.BadRequest);

            if (!request.NewPassword.Equals(request.ConfirmNewPassword, System.StringComparison.OrdinalIgnoreCase))
                throw new DomainException(ErrorCodeConsts.ConfirmNewPasswordNotInCorrect, ErrorMessageConsts.ConfirmNewPasswordNotInCorrect, System.Net.HttpStatusCode.BadRequest);

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            
            if (result == null)
                throw new DomainException(ErrorCodeConsts.ErrorWhenChangePassword, ErrorMessageConsts.ErrorWhenChangePassword, System.Net.HttpStatusCode.InternalServerError);
            if (!user.IsActive)
            {
                user.IsActive = true;
                await _userManager.UpdateAsync(user);
            }

            return new ChangePasswordResponseDto()
            {
                IsSuccess = result.Succeeded
            };
        }

        private async Task<TokenResponseDto> GenerateTokenAsync(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JwtSettings.Secret));

            var claims = await CreateClaims(user);
            var tokenExpireTime = DateTime.UtcNow.AddMinutes(2);
            var token = new JwtSecurityToken(
                issuer: AppSettings.JwtSettings.Issuer,
                audience: AppSettings.JwtSettings.Audience,
                claims: claims,
                expires: tokenExpireTime,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            return new TokenResponseDto
            {
                AccessToken = accessToken,
                ExpireTime = tokenExpireTime,
                RefreshToken = refreshToken,
                IsActive = user.IsActive
            };
        }

        private async Task<List<Claim>> CreateClaims(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("DisplayName", user.DisplayName),
                new Claim("IsActive", user.IsActive.ToString()),
                new Claim("ApartmentBuilding", user.DisplayName),
            };
            var roleNames = await _userManager.GetRolesAsync(user);
            var roleName = roleNames.FirstOrDefault();

            if (string.IsNullOrEmpty(roleName)) return claims;

            claims.AddRange(roleNames.Select(role => new Claim(ClaimTypes.Role, role)));
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null) return claims;
            var allClaims = await _roleManager.GetClaimsAsync(role);

            if (allClaims == null) return claims;
            claims.AddRange(allClaims.Select(claim => new Claim(claim.Type, claim.Value, claim.ValueType, claim.Issuer)));   
            return claims;
        }

        private HashToken CreateHasToken(string token)
        {
            var salt = TokenHelper.GenerateSalt();
            var hash = TokenHelper.HashToken(token, salt);
            return new HashToken()
            {
                Hash = hash,
                Salt = salt
            };
        }
        
        private bool VerifyHasToken(string token, string salt, string hash)
        {
            var incomingHash = TokenHelper.HashToken(token, salt);
            return incomingHash == hash;
        }

        
    }
    class HashToken
    {
        public string Salt { get; set; }
        public string Hash { get; set; }
    }
}
