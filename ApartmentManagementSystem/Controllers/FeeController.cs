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
        public async Task<IActionResult> GetFeeNotices(Guid apartmentId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response = _feeSerivce.GetFeeNotices(new RequestQueryBaseDto<Guid>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = apartmentId
            });
            return Ok(new ResponseData<IEnumerable<FeeNoticeDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
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
        public async Task<IActionResult> GetUtilityReading(Guid apartmentId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var utilityReadings = _feeSerivce.GetUtilityReadings(new RequestQueryBaseDto<Guid>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = apartmentId
            });
            return Ok(new ResponseData<IEnumerable<UtilityReadingDto>>(System.Net.HttpStatusCode.OK, utilityReadings.Items, null, new MetaData()
            {
                Page = page,
                Total = utilityReadings.Totals,
                PerPage = limit
            }));
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