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
    [Route("{appartmentBuildingId}/fee-configuration")]
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
        public async Task<IActionResult> GetFeeTypes([FromRoute] string appartmentBuildingId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response = _feeConfigurationService.GetFeeTypes(new RequestQueryBaseDto<string>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = appartmentBuildingId
            });
            return Ok(new ResponseData<IEnumerable<FeeTypeDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
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
    }
}