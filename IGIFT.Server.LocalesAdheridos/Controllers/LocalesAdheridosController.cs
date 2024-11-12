using IGift.Application.Requests.LocalesAdheridos.Query;
using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGIFT.Server.LocalesAdheridos.Controllers
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
