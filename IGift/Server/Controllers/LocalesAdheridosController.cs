using Microsoft.AspNetCore.Mvc;
using IGift.Server.Controllers.Base;
using IGift.Application.Requests.LocalesAdheridos.Query;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalesAdheridosController : BaseApiController<LocalesAdheridosController>
    {
        [HttpPost]
        public async Task<ActionResult> GetAll(GetAllLocalAdheridoQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
