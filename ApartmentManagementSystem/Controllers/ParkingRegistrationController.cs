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
    [ApiController]
    [Route("{appartmentBuildingId}/[controller]")]
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

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ParkingRegistrationDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> GetParkingRegistrations(Guid appartmentId)
        {
            var response = await _parkingRegistrationService.GetParkingRegistrations(appartmentId);
            return Ok(new ResponseData<IEnumerable<ParkingRegistrationDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.ReadWrite)]
        public async Task<IActionResult> CreateParkingRegistrations([FromBody] ParkingRegistrationDto request)
        {
            await _parkingRegistrationService.CreateParkingRegistration(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}