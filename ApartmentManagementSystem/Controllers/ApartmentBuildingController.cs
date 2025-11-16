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
        public async Task<IActionResult> GetApartmentBuildings()
        {
            var response = _apartmentBuildingService.GetApartmentBuildings();
            return Ok(new ResponseData<IEnumerable<ApartmentBuildingDto>>(System.Net.HttpStatusCode.OK, response, null, null));
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
