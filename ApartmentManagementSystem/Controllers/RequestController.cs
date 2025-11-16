using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("{appartmentBuildingId}/request")]
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
        public async Task<IActionResult> GetRequests([FromRoute] string appartmentBuildingId)
        {
            var response = _requestService.GetRequests(new Guid(appartmentBuildingId));
            return Ok(new ResponseData<IEnumerable<RequestDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet("{id:Guid}")]
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

        [HttpPost("feedback")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> CreateFeedBack([FromBody] FeedbackDto request)
        {
            await _requestService.CreateOrUpdateFeedback(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut("feedback")]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = RequestPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateFeedBack([FromBody] FeedbackDto request)
        {
            await _requestService.CreateOrUpdateFeedback(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}