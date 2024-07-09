using IGift.Application.MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseApiController<NotificationController>
    {
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll(string query)
        {
            try
            {
                var query2 = new GetAllNotificationQuery(query);
                var response = await _mediator.Send(query2);
                return Ok(response);
            }
            catch (Exception e)
            {

            }
            return BadRequest();

        }
    }
}
