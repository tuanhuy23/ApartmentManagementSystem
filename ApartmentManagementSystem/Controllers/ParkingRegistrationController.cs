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
    [ApiController]
    [Route("{apartmentBuildingId}/parking-registration")]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class ParkingRegistrationController : ControllerBase
    {
        private readonly IParkingRegistrationService _parkingRegistrationService;
        public ParkingRegistrationController(IParkingRegistrationService parkingRegistrationService)
        {
            _parkingRegistrationService = parkingRegistrationService;
        }

        [HttpGet("{appartmentId:Guid}")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ParkingRegistrationDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> GetParkingRegistrations(Guid appartmentId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response = _parkingRegistrationService.GetParkingRegistrations(new RequestQueryBaseDto<Guid>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = appartmentId
            });
            return Ok(new ResponseData<IEnumerable<ParkingRegistrationDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }
        
        [HttpGet("detail/{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<ParkingRegistrationDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.Read)]
        public async Task<IActionResult> GetParkingRegistration(Guid id)
        {
            var parkingRegistration = await _parkingRegistrationService.GetParkingRegistration(id);
            return Ok(new ResponseData<ParkingRegistrationDto>(System.Net.HttpStatusCode.OK, parkingRegistration, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> CreateParkingRegistrations([FromBody] ParkingRegistrationDto request)
        {
            await _parkingRegistrationService.CreateOrUpdateParkingRegistration(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateParkingRegistrations([FromBody] ParkingRegistrationDto request)
        {
            await _parkingRegistrationService.CreateOrUpdateParkingRegistration(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteParkingRegistrations([FromBody] List<string> request)
        {
            await _parkingRegistrationService.DeleteParkingRegistration(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}