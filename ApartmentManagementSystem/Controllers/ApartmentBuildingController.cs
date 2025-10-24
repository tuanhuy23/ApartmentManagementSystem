using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Impls;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApartmentBuildingController : ControllerBase
    {
        private readonly IApartmentBuildingService _apartmentBuildingService;
        public ApartmentBuildingController(IApartmentBuildingService apartmentBuildingService)
        {
            _apartmentBuildingService = apartmentBuildingService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ApartmentBuildingDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = "Permissions.ApartmentBuildingPermissions.Read")]
        public async Task<IActionResult> GetApartmentBuildings()
        {
            var response = _apartmentBuildingService.GetApartmentBuildings();
            return Ok(new ResponseData<IEnumerable<ApartmentBuildingDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = "Permissions.ApartmentBuildingPermissions.ReadWrite")]
        public async Task<IActionResult> CreateApartmentBuilding(CreateApartmentBuildingDto request)
        {
            await _apartmentBuildingService.CreateApartmentBuilding(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}
