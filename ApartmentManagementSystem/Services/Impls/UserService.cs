using ApartmentManagementSystem.DbContext;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Services.Impls
{
    class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthenticationDbContext _authenticationDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<AppUser> userManager, AuthenticationDbContext authenticationDbContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _authenticationDbContext = authenticationDbContext;
            _roleManager = roleManager;
        }
        public async Task<UserDto> CreateOrUpdateUser(CreateOrUpdateUserRequestDto request)
        {
            var result = new UserDto()
            {
                UserName = request.UserName,
                DisplayName = request.DisplayName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                Possition = request.Possition,
            };
            if (string.IsNullOrEmpty(request.UserId))
            {
                var appUser = new AppUser()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    PhoneNumber = request.PhoneNumber,
                    Position = request.Possition,
                };
                var resultUser = await _userManager.CreateAsync(appUser);
                if (!resultUser.Succeeded)
                    throw new DomainException(ErrorCodeConsts.ErrorWhenCreateUser, ErrorCodeConsts.ErrorWhenCreateUser, System.Net.HttpStatusCode.NotFound);

                var userNew = await _userManager.FindByEmailAsync(request.Email);
                var role = await _roleManager.FindByIdAsync(request.RoleId);
                if (role == null)
                    throw new DomainException(ErrorCodeConsts.RoleNotFound, ErrorCodeConsts.RoleNotFound, System.Net.HttpStatusCode.NotFound);

                await _userManager.AddToRoleAsync(userNew, role.Name);
                result.UserId = userNew.Id;
                result.RoleName = role.Name;
                return result;
            }
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorCodeConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            user.DisplayName = request.DisplayName;
            return null;

        }

        public Task<DeleteUserResponseDto> DeleteUsers(IEnumerable<string> userIds)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorCodeConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            
            var roleNames = await _userManager.GetRolesAsync(user);
            var roleName = roleNames.FirstOrDefault();
            string roleId = string.Empty;
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                roleId = role.Id;
            }

            UserDto userDto = new UserDto()
            {
                Email = user.Email,
                UserId = user.Id,
                RoleName = roleName,
                RoleId = roleId,
                DisplayName = user.DisplayName,
                Possition = user.Position,
                UserName = user.UserName
            };
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetUsers(string appartmentId = "")
        {
            var users = await _userManager.Users.ToListAsync();
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = new UserDto()
                {
                    Email = user.Email,
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    Possition = user.Position,
                    UserName = user.UserName
                };
                var roles = await _userManager.GetRolesAsync(user);
                userDto.RoleName = roles.FirstOrDefault();
                userDtos.Add(userDto);
            }
            return userDtos;
        }
    }
}
