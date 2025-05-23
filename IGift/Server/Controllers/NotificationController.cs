﻿using IGift.Application.CQRS.Notifications.Command;
using IGift.Application.CQRS.Notifications.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : BaseApiController<NotificationController>
    {
        [HttpPost("GetAll")]
        public async Task<ActionResult> GetAll(GetAllNotificationQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost("SaveNotificationAsync")]
        public async Task<ActionResult> SaveNotificationAsync(SaveNotificationCommand command)
        {
            return null;
        }
    }
}