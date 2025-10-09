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
        public TokenService(UserManager<AppUser> userManager, AuthenticationDbContext authenticationDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _authenticationDbContext = authenticationDbContext;
            _roleManager = roleManager;
        }
        public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null) throw new DomainException(ErrorCodeConsts.UserNameOrPasswordNotInCorrect, ErrorMessageConsts.UserNameOrPasswordNotInCorrect, 400);

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                throw new DomainException(ErrorCodeConsts.UserNameOrPasswordNotInCorrect, ErrorMessageConsts.UserNameOrPasswordNotInCorrect, 400);

            var tokenResponse = await GenerateTokenAsync(user);
            var hashToken = HashToken(tokenResponse.RefreshToken);

            var entityRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                TokenHash = hashToken.Hash,
                TokenSalt = hashToken.Salt,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
            };
            _authenticationDbContext.RefreshToken.Add(entityRefreshToken);
            await _authenticationDbContext.SaveChangesAsync();
            return tokenResponse;
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var storedToken = await _authenticationDbContext.RefreshToken.FirstOrDefaultAsync(x => x.TokenHash == HashToken(refreshToken).Hash);
            if (storedToken == null) throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, 400);
            storedToken.Revoked = DateTime.UtcNow;
            await _authenticationDbContext.SaveChangesAsync();
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string token)
        {
            var hasToken = HashToken(token);
            var storedToken = await _authenticationDbContext.RefreshToken.FirstOrDefaultAsync(x => x.TokenHash == hasToken.Hash);

            if (storedToken == null || storedToken.IsExpired || storedToken.IsRevoked)
                throw new DomainException(ErrorCodeConsts.RefreshTokenInvalid, ErrorMessageConsts.RefreshTokenInvalid, 400);

            var user = await _userManager.FindByIdAsync(storedToken.UserId);

            if (user == null) throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, 404);
            storedToken.Revoked = DateTime.UtcNow;

            var tokenResponse = await GenerateTokenAsync(user);
            var hashToken = HashToken(tokenResponse.RefreshToken);

            var entityRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                TokenHash = hashToken.Hash,
                TokenSalt = hashToken.Salt,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
            };
            _authenticationDbContext.RefreshToken.Add(entityRefreshToken);
            storedToken.ReplacedByTokenHash = hashToken.Hash;
            await _authenticationDbContext.SaveChangesAsync();
            return tokenResponse;
        }

        private async Task<TokenResponseDto> GenerateTokenAsync(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.JwtSettings.Secret));

            var claims = await CreateClaims(user);
            var tokenExpireTime = DateTime.UtcNow.AddHours(1);
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
            };
        }

        private async Task<List<Claim>> CreateClaims(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
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
        
        private HashToken HashToken(string token)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA256();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(token + salt)));
            return new HashToken()
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
    class HashToken
    {
        public string Salt { get; set; }
        public string Hash { get; set; }
    }
}
