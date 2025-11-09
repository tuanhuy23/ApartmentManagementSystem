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
    [Route("{appartmentBuildingId}/fee")]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class FeeController : ControllerBase
    {
        private readonly IFeeService _feeSerivce;
        public FeeController(IFeeService feeSerivce)
        {
            _feeSerivce = feeSerivce;
        }

        [HttpGet("{apartmentId:Guid}")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<FeeNoticeDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.Read)]
        public async Task<IActionResult> GetFeeNotices(Guid apartmentId)
        {
            var response = await _feeSerivce.GetFeeNotices(apartmentId);
            return Ok(new ResponseData<IEnumerable<FeeNoticeDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<FeeNoticeDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.Read)]
        public async Task<IActionResult> GetFeeNotice([FromQuery] Guid id)
        {
            var feeNoticeDetail = await _feeSerivce.GetFeeDetail(id);
            return Ok(new ResponseData<FeeNoticeDto>(System.Net.HttpStatusCode.OK, feeNoticeDetail, null, null));
        }

        [HttpGet("utility-reading/{apartmentId:Guid}")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<UtilityReadingDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.Read)]
        public async Task<IActionResult> GetUtilityReading(Guid apartmentId)
        {
            var utilityReadings = await _feeSerivce.GetUtilityReadings(apartmentId);
            return Ok(new ResponseData<IEnumerable<UtilityReadingDto>>(System.Net.HttpStatusCode.OK, utilityReadings, null, null));
        }
        
        [HttpPost()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.Read)]
        public async Task<IActionResult> CreateFeeNotice(CreateOrUpdateFeeNoticeDto request)
        {
            await _feeSerivce.CreateFeeNotice(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.Read)]
        public async Task<IActionResult> UpdateFeeNotice(CreateOrUpdateFeeNoticeDto request)
        {
            await _feeSerivce.CreateFeeNotice(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}