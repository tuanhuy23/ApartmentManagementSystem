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
    [Route("{appartmentBuildingId}/billing-cycle-setting")]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class BillingCycleSettingController : ControllerBase
    {
        private readonly IBillingCycleSettingService _billingCycleSettingService;
        public BillingCycleSettingController(IBillingCycleSettingService billingCycleSettingService)
        {
            _billingCycleSettingService = billingCycleSettingService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<BillingCycleSettingDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> GetBillingCycleSetting([FromRoute] string appartmentBuildingId)
        {
            var response = await _billingCycleSettingService.GetBillingCycleSetting(appartmentBuildingId);
            return Ok(new ResponseData<BillingCycleSettingDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeConfigurationPermissions.ReadWrite)]
        public async Task<IActionResult> CreateApartment([FromBody] BillingCycleSettingDto request)
        {
            await _billingCycleSettingService.CreateBillingCycleSetting(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}