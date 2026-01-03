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
    [Route("{apartmentBuildingId}/fee")]
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
        [Authorize(Policy = FeeNoticePermissions.ReadWrite)]
        public async Task<IActionResult> CreateFeeNotice(CreateOrUpdateFeeNoticeDto request)
        {
            await _feeSerivce.CreateFeeNotice(new List<CreateOrUpdateFeeNoticeDto>(){request});
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{id:Guid}/cancel-fee")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.ReadWrite)]
        public async Task<IActionResult> CancelFeeNotice(Guid id)
        {
            await _feeSerivce.CancelFeeNotice(id);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("{id:Guid}/update-payment-status-fee")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.ReadWrite)]
        public async Task<IActionResult> UpdatePaymentStatusFeeNotice(Guid id)
        {
            await _feeSerivce.UpdatePaymentStatusFeeNotice(id);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = FeeNoticePermissions.ReadWrite)]
        public async Task<IActionResult> DeleteFeeNotice([FromBody] List<string> request)
        {
            await _feeSerivce.DeletFeeNotice(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpGet("download-excel-template")]
        [Authorize(Policy = FeeNoticePermissions.Read)]
        public async Task<IActionResult> DownloadExcelTemplate([FromRoute] string apartmentBuildingId)
        {
            string fileName = "Sub-ProjectSetUpDefect/Fee-ExcelTemplate.xlsx";
            string sheetName = "Data";
            var excelData = _feeSerivce.DownloadExcelTemplate(fileName, sheetName, apartmentBuildingId);
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }

        [HttpPost("import")]
        [Authorize(Policy = FeeNoticePermissions.ReadWrite)]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<ImportFeeNoticeResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ImportFeeNotice([FromRoute] string apartmentBuildingId, IFormFile file)
        {
            var excelData = await _feeSerivce.ImportFeeNoticeResult(apartmentBuildingId, file);
            return Ok(new ResponseData<IEnumerable<ImportFeeNoticeResult>>(System.Net.HttpStatusCode.OK, excelData, null, null));
        }
    }
}