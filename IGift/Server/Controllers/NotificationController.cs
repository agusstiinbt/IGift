using IGift.Application.CQRS.Notifications.Query;
using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseApiController<NotificationController>
    {
        [HttpPost]
        public async Task<ActionResult> GetAll(GetAllNotificationQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
