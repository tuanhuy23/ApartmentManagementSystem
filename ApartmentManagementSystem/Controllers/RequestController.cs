using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("{apartmentBuildingId}/request")]
    [Authorize]
    [ApiExceptionFilter]
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<RequestDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.Read)]
        public async Task<IActionResult> GetRequests([FromRoute] string apartmentBuildingId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response = await _requestService.GetRequests(new RequestQueryBaseDto<Guid>()
            {
                Filters = filters,
                Page = page,
                Sorts = sorts,
                PageSize = limit,
                Request = new Guid(apartmentBuildingId)
            });
            return Ok(new ResponseData<IEnumerable<RequestDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpGet("{requestId:Guid}")]
        [ProducesResponseType(typeof(ResponseData<RequestDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.Read)]
        public async Task<IActionResult> GetRequest(Guid requestId)
        {
            var response = _requestService.GetRequest(requestId);
            return Ok(new ResponseData<RequestDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> CreateRequest([FromBody] RequestDto request)
        {
            await _requestService.CreateOrUpdateRequest(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateRequest([FromBody] RequestDto request)
        {
            await _requestService.CreateOrUpdateRequest(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteRequest([FromBody] List<string> request)
        {
            await _requestService.DeleteRequest(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPost("request-action")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> CreateOrUpdateRequestAction([FromBody] RequestHistoryDto request)
        {
             await _requestService.CreateOrUpdateRequestAction(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("request-action")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateFeedBack([FromBody] RequestHistoryDto request)
        {
            await _requestService.CreateOrUpdateRequestAction(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("status")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateStatusAndAssignRequest([FromBody] UpdateStatusAndAssignRequestDto request)
        {
            await _requestService.UpdateStatusAndAssignRequest(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("ratting")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> RattingRequest([FromBody] RattingRequestDto request)
        {
            await _requestService.RattingRequest(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
        [HttpGet("user-handler")]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> GetUserHandler([FromRoute] string apartmentBuildingId)
        {
            var result = await _requestService.GetUserHandlers(apartmentBuildingId);
            return Ok(new ResponseData<IEnumerable<UserDto>>(System.Net.HttpStatusCode.OK, result, null, null));
        }
    }
}