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
        public async Task<IActionResult> GetAnnouncements([FromRoute] string appartmentBuildingId)
        {
            var response = _notificationService.GetAnnouncements(new Guid(appartmentBuildingId));
            return Ok(new ResponseData<IEnumerable<AnnouncementDto>>(System.Net.HttpStatusCode.OK, response, null, null));
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseData<AnnouncementDto>), StatusCodes.Status200OK)]
        [Authorize(Policy = NotificationPermissions.Read)]
        public async Task<IActionResult> GetAnnouncement(Guid requestId)
        {
            var response = _notificationService.GetAnnouncement(requestId);
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

    }
}