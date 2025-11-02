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
    public class ApartmentController : ControllerBase
    {
        private readonly IApartmentService _apartmentService;
        public ApartmentController(IApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ApartmentDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetApartments([FromRoute] string appartmentBuildingId)
        {
            var response = await _apartmentService.GetApartments(appartmentBuildingId);
            return Ok(new ResponseData<IEnumerable<ApartmentDto>>(System.Net.HttpStatusCode.OK, response, null, null));
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
    }
}