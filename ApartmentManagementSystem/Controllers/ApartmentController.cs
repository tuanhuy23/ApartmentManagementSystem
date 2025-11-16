using ApartmentManagementSystem.Consts.Permissions;
using ApartmentManagementSystem.Dtos;
using ApartmentManagementSystem.EF.Context;
using ApartmentManagementSystem.Exceptions;
using ApartmentManagementSystem.Filters;
using ApartmentManagementSystem.Response;
using ApartmentManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("{apartmentId:Guid}/residents")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ResidentDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetResidents(Guid apartmentId)
        {
            var response =  _residentService.GetResidents(apartmentId);
            return Ok(new ResponseData<IEnumerable<ResidentDto>>(System.Net.HttpStatusCode.OK, response, null, null));
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
    }
}