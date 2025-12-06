using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Filters;
using ApartmentManagementSystem.Identity;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApartmentManagementSystem.Controllers
{
    [Route("{appartmentBuildingId}/role")]
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
            var response = await _roleService.GetRoles(new RequestQueryBaseDto<string>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = appartmentBuildingId
            });
            return Ok(new ResponseData<IEnumerable<RoleDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpGet("{roleId}")]
        [ProducesResponseType(typeof(ResponseData<RoleDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.Read)]
        public async Task<IActionResult> GetRole([FromQuery] string roleId)
        {
            var response = await _roleService.GetRole(roleId);
            return Ok(new ResponseData<RoleDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet("permissions")]
        [ProducesResponseType(typeof(ResponseData<PermissionInfo>), StatusCodes.Status200OK)]
        [Authorize(Policy = UserPermissions.Read)]
        public async Task<IActionResult> GetPermission()
        {
            var response = await _roleService.GetPermissionInfos();
            return Ok(new ResponseData<IEnumerable<PermissionInfo>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<RoleDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.ReadWrite)]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto request)
        {
            var response = await _roleService.CreateOrUpdateRole(request);
            return Ok(new ResponseData<RoleDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<RoleDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.ReadWrite)]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDto request)
        {
            var response = await _roleService.CreateOrUpdateRole(request);
            return Ok(new ResponseData<RoleDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<DeleteRoleResponse>), StatusCodes.Status200OK)]
        [Authorize(Policy = RolePermissions.ReadWrite)]
        public async Task<IActionResult> DeleteRole([FromBody] List<string> request)
        {
            var response = await _roleService.DeleteRoles(request);
            return Ok(new ResponseData<DeleteRoleResponse>(System.Net.HttpStatusCode.OK, response, null, null));
        }
    }
}
