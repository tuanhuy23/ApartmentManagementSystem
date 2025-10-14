using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExceptionFilter]
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
        [ProducesResponseType(typeof(ResponseData<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser([FromQuery] CreateOrUpdateUserRequestDto request)
        {
            var response = await _userService.DeleteUsers(request);
            return Ok(new ResponseData<UserDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }
    }
}