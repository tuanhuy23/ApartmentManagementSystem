using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Filters;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApartmentManagementSystem.Controllers
{
    [Route("{appartmentBuildingId}/user")]
    [ApiController]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.Read)]
        public async Task<IActionResult> Get([FromRoute] string appartmentBuildingId, [FromQuery(Name = "filters")] string? filtersJson,
            [FromQuery(Name = "sorts")] string? sortsJson, [FromHeader] int page = 1, [FromHeader] int limit = 20)
        {
            List<FilterQuery> filters = new List<FilterQuery>();
            if (!string.IsNullOrEmpty(filtersJson))
            {
                filters = JsonConvert.DeserializeObject<List<FilterQuery>>(filtersJson);
            }

            List<SortQuery> sorts = new List<SortQuery>();
            if (!string.IsNullOrEmpty(sortsJson))
            {
                sorts = JsonConvert.DeserializeObject<List<SortQuery>>(sortsJson);
            }
            var response = await _userService.GetUsers(new RequestQueryBaseDto<string>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = appartmentBuildingId
            });
            return Ok(new ResponseData<IEnumerable<UserDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.Read)]
        public async Task<IActionResult> GetUser([FromQuery] string userId)
        {
            var response = await _userService.GetUser(userId);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.ReadWrite)]
        public async Task<IActionResult> CreateUser([FromBody] CreateOrUpdateUserRequestDto request)
        {
            var response = await _userService.CreateOrUpdateUser(request);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateUser([FromBody] CreateOrUpdateUserRequestDto request)
        {
            var response = await _userService.CreateOrUpdateUser(request);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<DeleteUserResponseDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteUser([FromBody] List<string> request)
        {
            var response = await _userService.DeleteUsers(request);
            return Ok(new ResponseData<DeleteUserResponseDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }
    }
}