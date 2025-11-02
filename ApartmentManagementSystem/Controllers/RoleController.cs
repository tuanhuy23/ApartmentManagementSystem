using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Filters;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Route("{appartmentBuildingId}/[controller]")]
    [ApiController]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<RoleDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.Read)]
        public async Task<IActionResult> Get()
        {
            var response = await _roleService.GetRoles();
            return Ok(new ResponseData<IEnumerable<RoleDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<RoleDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.ReadWrite)]
        public async Task<IActionResult> CreateUser([FromBody] RoleDto request)
        {
            var response = await _roleService.CreateOrUpdateRole(request);
            return Ok(new ResponseData<RoleDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<RoleDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.ReadWrite)]
        public async Task<IActionResult> UpdateUser([FromBody] RoleDto request)
        {
            var response = await _roleService.CreateOrUpdateRole(request);
            return Ok(new ResponseData<RoleDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<DeleteRoleResponse>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.ReadWrite)]
        public async Task<IActionResult> DeleteUser([FromBody] List<string> request)
        {
            var response = await _roleService.DeleteRoles(request);
            return Ok(new ResponseData<DeleteRoleResponse>(System.Net.HttpStatusCode.OK, response, null, null));
        }
    }
}
