using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExceptionFilter]
    [Authorize]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var response = await _userService.GetUsers();
            return Ok(new ResponseData<IEnumerable<UserDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser([FromQuery] string userId)
        {
            var response = await _userService.GetUser(userId);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUser([FromBody] CreateOrUpdateUserRequestDto request)
        {
            var response = await _userService.CreateOrUpdateUser(request);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser([FromBody] CreateOrUpdateUserRequestDto request)
        {
            var response = await _userService.CreateOrUpdateUser(request);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<DeleteUserResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser([FromBody] List<string> request)
        {
            var response = await _userService.DeleteUsers(request);
            return Ok(new ResponseData<DeleteUserResponseDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }
    }
}