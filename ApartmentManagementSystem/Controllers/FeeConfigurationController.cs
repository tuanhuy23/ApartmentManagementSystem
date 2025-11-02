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
        public async Task<IActionResult> CreateFeeType([FromBody] CreateFeeTypeDto request)
        {
            await _feeConfigurationService.CreateFeeType(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpGet("{feeTypeId:Guid}/getFeeRateConfigs")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<FeeRateConfigDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = ApartmentPermissions.Read)]
        public async Task<IActionResult> GetFeeRateConfigs(Guid feeTypeId)
        {
            var response = await _feeConfigurationService.GetFeeRateConfigs(feeTypeId);
            return Ok(new ResponseData<IEnumerable<FeeRateConfigDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost("{feeTypeId:Guid}/createFeeRateConfigs")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> CreateFeeRateConfig(Guid feeTypeId, [FromBody] CreateFeeRateConfigDto request)
        {
            await _feeConfigurationService.CreateFeeRateConfig(request, feeTypeId);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{feeTypeId:Guid}/updateFeeRateConfigs")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateFeeRateConfig(Guid feeTypeId, [FromBody] UpdateFeeRateConfigDto request)
        {
            await _feeConfigurationService.UpdateFeeRateConfig(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{feeTypeId:Guid}/deleteFeeRateConfigs/{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteFeeRateConfig(Guid feeTypeId, Guid id)
        {
            await _feeConfigurationService.DeleteFeeRateConfig(id);
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