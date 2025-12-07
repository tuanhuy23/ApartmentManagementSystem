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
        public async Task<IActionResult> CreateApartmentBuilding([FromBody] CreateOrUpdateApartmentBuildingDto request)
        {
            await _apartmentBuildingService.CreateOrUpdateApartmentBuilding(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<ApartmentBuildingDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentBuildingPermissions.ReadWrite)]
        public async Task<IActionResult> GetApartmentBuilding(Guid id)
        {
            var apartmentBuilding =  await _apartmentBuildingService.GetApartmentBuilding(id);
            return Ok(new ResponseData<ApartmentBuildingDto>(System.Net.HttpStatusCode.OK, apartmentBuilding, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentBuildingPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateApartmentBuilding([FromBody] CreateOrUpdateApartmentBuildingDto request)
        {
            await _apartmentBuildingService.CreateOrUpdateApartmentBuilding(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{id:Guid}/status")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentBuildingPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateApartmentBuildingStatus(Guid id, [FromBody] UpdateStatusApartmentBuildingDto request)
        {
            await _apartmentBuildingService.UpdateApartmentBuildingStatus(id, request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentBuildingPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteApartmentBuilding([FromBody] List<string> request)
        {
            await _apartmentBuildingService.DeleteApartmentBuilding(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}
