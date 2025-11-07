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
    public class FeeConfigurationController : ControllerBase
    {
        private readonly IFeeConfigurationService _feeConfigurationService;
        public FeeConfigurationController(IFeeConfigurationService feeConfigurationService)
        {
            _feeConfigurationService = feeConfigurationService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<FeeTypeDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.Read)]
        public async Task<IActionResult> GetFeeTypes([FromRoute] string appartmentBuildingId)
        {
            var response = await _feeConfigurationService.GetFeeTypes(appartmentBuildingId);
            return Ok(new ResponseData<IEnumerable<FeeTypeDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<FeeTypeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.Read)]
        public async Task<IActionResult> GetFeeType(Guid id)
        {
            var response = await _feeConfigurationService.GetFeeType(id);
            return Ok(new ResponseData<FeeTypeDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> CreateFeeType([FromBody] CreateOrUpdateFeeTypeDto request)
        {
            await _feeConfigurationService.CreateOrUpdateFeeType(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateFeeType([FromBody] CreateOrUpdateFeeTypeDto request)
        {
            await _feeConfigurationService.CreateOrUpdateFeeType(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{id:Guid}/acticeFeeType")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> ActiveFeeType(Guid id)
        {
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
        
        [HttpPut("{feeTypeId:Guid}/acticeFeeRateConfig/{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> ActiveFeeRateConfig(Guid id)
        {
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}