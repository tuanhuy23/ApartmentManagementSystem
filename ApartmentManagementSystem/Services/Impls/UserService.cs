using ApartmentManagementSystem.Common;
using ApartmentManagementSystem.Consts;
using ApartmentManagementSystem.DbContext;
using ApartmentManagementSystem.DbContext.Entity;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagementSystem.Services.Impls
{
    class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly HttpContext _httpContext = null;
        private readonly IEmailService _emailService;
        public UserService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            if (httpContextAccessor.HttpContext != null)
            {
                _httpContext = httpContextAccessor.HttpContext;
            }
            _emailService = emailService;
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
            };

            IdentityRole role = null;
            if (!string.IsNullOrEmpty(request.RoleId))
            {
                role = await _roleManager.FindByIdAsync(request.RoleId);

                if (role == null)
                    throw new DomainException(ErrorCodeConsts.RoleNotFound, ErrorMessageConsts.RoleNotFound, System.Net.HttpStatusCode.NotFound);
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                var appUser = new AppUser()
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    PhoneNumber = request.PhoneNumber,
                    AppartmentBuildingId = request.AppartmentBuildingId,
                    ApartmentId = request.ApartmentId
                };       
                IdentityResult resultUser = await _userManager.CreateAsync(appUser, request.Password);
                if (!resultUser.Succeeded)
                    throw new DomainException(ErrorCodeConsts.ErrorCreatingUser, ErrorMessageConsts.ErrorCreatingUser, System.Net.HttpStatusCode.NotFound);

                var userNew = await _userManager.FindByEmailAsync(request.Email);
                await _userManager.AddToRoleAsync(userNew, role.Name);
                result.UserId = userNew.Id;
                result.RoleName = role.Name;
                //TODO: send email
                //await _emailService.SendEmailAsync(request.Email, "Password Active Account", $"<h5>This is my password. Please login to acction. Password : {request.Password}</h5>");
                return result;
            }
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            user.DisplayName = request.DisplayName;
            user.PhoneNumber = request.PhoneNumber;
            user.Email = request.Email;
           
            if (role != null)
            {
                var roleCurrents = await _userManager.GetRolesAsync(user);
                var roleNameCur = roleCurrents.FirstOrDefault();
                if (!string.IsNullOrEmpty(roleNameCur))
                {
                    await _userManager.RemoveFromRoleAsync(user, roleNameCur);
                }
                await _userManager.AddToRoleAsync(user, role.Name);
                result.RoleId = role.Id;
            }
            await _userManager.UpdateAsync(user);
            result.UserId = result.UserId;
            result.AppartmentBuildingId = user.AppartmentBuildingId;
            return result;

        }

        public async Task<DeleteUserResponseDto> DeleteUsers(IEnumerable<string> userIds)
        {
            var result = new DeleteUserResponseDto()
            {
                UserIdsDeleteSuccess = new List<string>(),
                UserIdsDeleteError = new List<string>()
            };
            foreach (var id in userIds)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    var roleNames = await _userManager.GetRolesAsync(user);
                    var roleName = roleNames.FirstOrDefault();
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                    await _userManager.DeleteAsync(user);
                    result.UserIdsDeleteSuccess.Add(id);
                }
                else
                {
                    result.UserIdsDeleteError.Add(id);
                }
            }
            return result;
        }

        public async Task<UserDto> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            
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
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
            return userDto;
        }

        public async Task<Pagination<UserDto>> GetUsers(RequestQueryBaseDto<string> request)
        {
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            if (accountInfo == null) throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            var users  = _userManager.Users.Where(u => request.Request.Equals(u.AppartmentBuildingId)).ToList();;
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                if (user.UserName.Equals("superadmin@gmail.com")) continue;
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();
                if (roleName == null) continue;
                if (roleName.Equals(RoleDefaulConsts.Management) || roleName.Equals(RoleDefaulConsts.Resident)) continue;
                var userDto = new UserDto()
                {
                    Email = user.Email,
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber
                };
                userDto.RoleName = roleName;
                userDtos.Add(userDto);
            }
            var userQuery = userDtos.AsQueryable();
            if (request.Filters!= null && request.Filters.Any())
            {
                userQuery = FilterHelper.ApplyFilters(userQuery, request.Filters);
            }
            if (request.Sorts!= null && request.Sorts.Any())
            {
                userQuery = SortHelper.ApplySort(userQuery, request.Sorts);
            }
            return new Pagination<UserDto>()
            {
                Items = userQuery.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList(),
                Totals = userQuery.Count()
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers(string apartmentBuidlingId)
        {
            var accountInfo = IdentityHelper.GetIdentity(_httpContext);
            if (accountInfo == null) throw new DomainException(ErrorCodeConsts.UserNotFound, ErrorMessageConsts.UserNotFound, System.Net.HttpStatusCode.NotFound);
            var users  = _userManager.Users.Where(u => apartmentBuidlingId.Equals(u.AppartmentBuildingId)).ToList();;
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                if (user.UserName.Equals("superadmin@gmail.com")) continue;
                var roles = await _userManager.GetRolesAsync(user);
                var roleName = roles.FirstOrDefault();
                if (roleName == null) continue;
                if (roleName.Equals(RoleDefaulConsts.Management) || roleName.Equals(RoleDefaulConsts.Resident)) continue;
                var userDto = new UserDto()
                {
                    Email = user.Email,
                    UserId = user.Id,
                    DisplayName = user.DisplayName,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber
                };
                userDto.RoleName = roleName;
                userDtos.Add(userDto);
            }
           return userDtos;
        }
    }
    
}
