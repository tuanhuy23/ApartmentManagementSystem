using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Dtos.Base;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Filters;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApartmentManagementSystem.Controllers
{
    [ApiController]
    [Route("{appartmentBuildingId}/apartment")]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class ApartmentController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;
        private readonly IResidentService _residentService;
        public ApartmentController(IApartmentService apartmentService, IResidentService residentService)
        {
            _apartmentService = apartmentService;
            _residentService = residentService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ApartmentDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetApartments([FromRoute] string appartmentBuildingId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response = _apartmentService.GetApartments(new RequestQueryBaseDto<string>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = appartmentBuildingId
            });
            return Ok(new ResponseData<IEnumerable<ApartmentDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<ApartmentDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetApartment(Guid id)
        {
            var response = await _apartmentService.GetApartment(id);
            return Ok(new ResponseData<ApartmentDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentDto request)
        {
            await _apartmentService.CreateApartment(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
        
        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateApartment([FromBody] UpdateApartmentDto request)
        {
            await _apartmentService.UpdateApartment(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteApartment(Guid id)
        {
            await _apartmentService.DeleteApartment(id);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpGet("{apartmentId:Guid}/residents")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ResidentDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetResidents(Guid apartmentId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response =  _residentService.GetResidents(new RequestQueryBaseDto<Guid>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = apartmentId
            });
            return Ok(new ResponseData<IEnumerable<ResidentDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpGet("{apartmentId:Guid}/residents/detail/{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<ResidentDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetResident(Guid apartmentId, Guid id)
        {
            var response = await _residentService.GetResident(id, apartmentId);
            return Ok(new ResponseData<ResidentDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost("{apartmentId:Guid}/residents")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> CreateResident([FromBody] ResidentDto request)
        {
            await _residentService.CreateOrUpdateResident(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{apartmentId:Guid}/residents")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateResident([FromBody] ResidentDto request)
        {
            await _residentService.CreateOrUpdateResident(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete("{apartmentId:Guid}/residents")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteResident(Guid apartmentId, [FromBody] List<string> request)
        {
            await _residentService.DeleteResident(apartmentId, request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}