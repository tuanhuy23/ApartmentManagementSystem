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
    [Route("apartment-building")]
    [ApiController]
    [Authorize]
    [ApiExceptionFilter]
    public class ApartmentBuildingController : ControllerBase
    {
        private readonly IApartmentBuildingService _apartmentBuildingService;
        public ApartmentBuildingController(IApartmentBuildingService apartmentBuildingService)
        {
            _apartmentBuildingService = apartmentBuildingService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ApartmentBuildingDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentBuildingPermissions.Read)]
        public async Task<IActionResult> GetApartmentBuildings([FromQuery(Name = "filters")] string? filtersJson,
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
            var response = _apartmentBuildingService.GetApartmentBuildings(new RequestQueryBaseDto<object>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
            });
            return Ok(new ResponseData<IEnumerable<ApartmentBuildingDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentBuildingPermissions.ReadWrite)]
        public async Task<IActionResult> CreateApartmentBuilding([FromBody] CreateApartmentBuildingDto request)
        {
            await _apartmentBuildingService.CreateApartmentBuilding(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}
