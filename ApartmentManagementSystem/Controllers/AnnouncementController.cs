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
    [Route("{appartmentBuildingId}/announcement")]
    [Authorize]
    [ApiExceptionFilter] 
    [ServiceFilter(typeof(ApartmentBuildingValidationFilter))]

    public class AnnouncementController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public AnnouncementController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet()]
        [ProducesResponseType(typeof(ResponseData<IEnumerable<AnnouncementDto>>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.Read)]
        public async Task<IActionResult> GetAnnouncements([FromRoute] string appartmentBuildingId, [FromQuery(Name = "filters")] string? filtersJson,
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
            var response = _notificationService.GetAnnouncements(new RequestQueryBaseDto<Guid>()
            {
                Filters = filters,
                Sorts = sorts,
                Page = page,
                PageSize = limit,
                Request = new Guid(appartmentBuildingId)
            });
            return Ok(new ResponseData<IEnumerable<AnnouncementDto>>(System.Net.HttpStatusCode.OK, response.Items, null, new MetaData()
            {
                Page = page,
                Total = response.Totals,
                PerPage = limit
            }));
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<AnnouncementDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.Read)]
        public async Task<IActionResult> GetAnnouncement(Guid id)
        {
            var response = _notificationService.GetAnnouncement(id);
            return Ok(new ResponseData<AnnouncementDto>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.ReadWrite)]
        public async Task<IActionResult> CreateAnnouncement([FromBody] AnnouncementDto request)
        {
            await _notificationService.CreateOrUpdateAnnouncements(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.ReadWrite)]
        public async Task<IActionResult> UpdateAnnouncement([FromBody] AnnouncementDto request)
        {
            await _notificationService.CreateOrUpdateAnnouncements(request);
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ResponseData<>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.ReadWrite)]
        public async Task<IActionResult> DeleteAnnouncement([FromBody] List<string> request)
        {
            return Ok(new ResponseData<object>(System.Net.HttpStatusCode.OK, null, null, null));
        }
    }
}