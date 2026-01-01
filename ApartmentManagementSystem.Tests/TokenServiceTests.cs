using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.DbContext;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ApartmentManagementSystem.Tests
{
    [TestFixture]
    public class TokenServiceTests
    {
        private AuthenticationDbContext _dbContext;
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private ITokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AuthenticationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AuthenticationDbContext(options);

            var userStore = new UserStore<AppUser, AppRole, AuthenticationDbContext, string>(_dbContext);
            var roleStore = new RoleStore<AppRole, AuthenticationDbContext, string>(_dbContext);

            var userLogger = new Mock<ILogger<UserManager<AppUser>>>();
            var roleLogger = new Mock<ILogger<RoleManager<AppRole>>>();

            _userManager = new UserManager<AppUser>(userStore, null, new PasswordHasher<AppUser>(),
                null, null, null, null, null, userLogger.Object);
            _roleManager = new RoleManager<AppRole>(roleStore, null, null, null, roleLogger.Object);

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            AppSettings.JwtSettings = new JwtSettings
            {
                Audience = "ApartmentManagementSystemUser",
                Issuer = "ApartmentManagementSystem",
                Secret = "ThisIsAReallyStrongSecretKeyForJwt"
            };
            _tokenService = new TokenService(_userManager, _dbContext, _roleManager, _mockHttpContextAccessor.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _userManager?.Dispose();
            _roleManager?.Dispose();
        }

        private async Task<AppUser> SeedUserAsync(string username, string password)
        {
            var user = new AppUser
            {
                UserName = username,
                Email = $"{username}@test.com",
                Id = Guid.NewGuid().ToString(),
                DisplayName = "Test User",
                AppartmentBuildingId = Guid.NewGuid().ToString(),
                ApartmentId = Guid.NewGuid().ToString(),
                IsActive = true
            };
            await _userManager.CreateAsync(user, password);
            return user;
        }

        private void MockCurrentUser(string userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, "testuser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = principal };
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
        }


        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            await SeedUserAsync("user1", "Pass123!");
            var request = new LoginRequestDto { UserName = "user1", Password = "Pass123!" };

            var result = await _tokenService.LoginAsync(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(result.IsActive, Is.True);
        }

        [Test]
        public async Task LoginAsync_WrongPassword_ReturnsNull()
        {
            await SeedUserAsync("user2", "Pass123!");
            var request = new LoginRequestDto { UserName = "user2", Password = "WrongPass" };

            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () =>
            {
                await _tokenService.LoginAsync(request);
            });

            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.UserNameOrPasswordIncorrect));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.UserNameOrPasswordIncorrect));
        }

        [Test]
        public async Task LoginAsync_UserNotFound_ReturnsNull()
        {
            var request = new LoginRequestDto { UserName = "ghost_user", Password = "Any" };

            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () =>
            {
                await _tokenService.LoginAsync(request);
            });

            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.UserNameOrPasswordIncorrect));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.UserNameOrPasswordIncorrect));
        }

        [TestCase("", "Pass")]
        [TestCase("User", "")]
        [TestCase("User", null)]
        public async Task LoginAsync_InvalidInput_ReturnsNull(string? u, string? p)
        {
            var request = new LoginRequestDto { UserName = u, Password = p };

            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () =>
            {
                await _tokenService.LoginAsync(request);
            });

            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.UserNameOrPasswordIncorrect));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.UserNameOrPasswordIncorrect));
        }


        [Test]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewToken()
        {
            var user = await SeedUserAsync("refreshUser", "Pass123!");
            var idRefreshToken = Guid.NewGuid().ToString();
            var oldRefreshToken = "valid_refresh_token_123";
            var oldToken = $"{idRefreshToken}.{oldRefreshToken}";
            var salt = TokenHelper.GenerateSalt();
            var hash = TokenHelper.HashToken(oldRefreshToken, salt);
            _dbContext.RefreshToken.Add(new RefreshToken
            {
                Id = idRefreshToken,
                UserId = user.Id,
                TokenHash = hash,
                Expires = DateTime.UtcNow.AddDays(1),
                TokenSalt = salt,
            });
            await _dbContext.SaveChangesAsync();

            var result = await _tokenService.RefreshTokenAsync(oldToken);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RefreshToken, Is.Not.EqualTo(oldToken));
        }

        [Test]
        public async Task RefreshTokenAsync_TokenNotFound_ReturnsNull()
        {

            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () =>
            {
                await _tokenService.RefreshTokenAsync("fake_token_xyz");
            });

            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.RefreshTokenInvalid));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.RefreshTokenInvalid));
        }

        [Test]
        public async Task LogoutAsync_ValidToken_RevokesToken()
        {

            var user = await SeedUserAsync("logoutUser", "Pass123!");
            var idRefreshToken = Guid.NewGuid().ToString();
            var oldRefreshToken = "valid_refresh_token_123";
            var oldToken = $"{idRefreshToken}.{oldRefreshToken}";
            var salt = TokenHelper.GenerateSalt();
            var hash = TokenHelper.HashToken(oldRefreshToken, salt);

            _dbContext.RefreshToken.Add(new RefreshToken
            {
                Id = idRefreshToken,
                UserId = user.Id,
                TokenHash = hash,
                Expires = DateTime.UtcNow.AddDays(1),
                TokenSalt = salt,
            });
            await _dbContext.SaveChangesAsync();

            await _tokenService.LogoutAsync(oldToken);

            var tokenInDb = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.Id == idRefreshToken);
            Assert.That(tokenInDb, Is.Not.Null);
            Assert.That(tokenInDb.Revoked, Is.Not.Null);
        }

        [Test]
        public void LogoutAsync_InvalidToken_DoesNotThrow()
        {
            var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () => await _tokenService.LogoutAsync("non_existent_token"));
            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.RefreshTokenInvalid));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.RefreshTokenInvalid));
        }


        [Test]
        public async Task ChangePassword_Success_ReturnsTrue()
        {
            var user = await SeedUserAsync("changeUser", "OldPass123!");
            MockCurrentUser(user.Id);
            _tokenService = new TokenService(_userManager, _dbContext, _roleManager, _mockHttpContextAccessor.Object);
            var req = new ChangePasswordRequestDto
            {
                OldPassword = "OldPass123!",
                NewPassword = "NewPass1!",
                ConfirmNewPassword = "NewPass1!"
            };

            var result = await _tokenService.ChangePassword(req);

            Assert.That(result.IsSuccess, Is.True);
            var isPassChanged = await _userManager.CheckPasswordAsync(user, "NewPass1!");
            Assert.That(isPassChanged, Is.True);
        }

        [Test]
        public async Task ChangePassword_WrongOldPass_ReturnsFalse()
        {
            var user = await SeedUserAsync("changeUser2", "OldPass123!");
            MockCurrentUser(user.Id);
             _tokenService = new TokenService(_userManager, _dbContext, _roleManager, _mockHttpContextAccessor.Object);
            var req = new ChangePasswordRequestDto
            {
                OldPassword = "WrongPassword!",
                NewPassword = "NewPass1!",
                ConfirmNewPassword = "NewPass1!"
            };
             var exception = Assert.ThrowsAsync<Exceptions.DomainException>(async () => await _tokenService.ChangePassword(req));
            Assert.That(exception.Message, Is.EqualTo(ErrorMessageConsts.OldPasswordIncorrect));
            Assert.That(exception.Code, Is.EqualTo(ErrorCodeConsts.OldPasswordIncorrect));
        }
    }
}