using IGift.Application.MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseApiController<NotificationController>
    {
        [HttpGet("GetAll/{IdUser}")]//TODO usar FromRoute?
        public async Task<ActionResult> GetAll([FromRoute] string IdUser)
        {
            var query = new GetAllNotificationQuery(IdUser);
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
