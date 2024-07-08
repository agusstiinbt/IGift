using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseApiController<NotificationController>
    {

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {

        }
    }
}
