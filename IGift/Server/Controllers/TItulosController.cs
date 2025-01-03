using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TItulosController : BaseApiController<TItulosController>
    {
        [HttpGet("GetBarraHerramientasDesconectado")]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _mediator.Send(new GetBarraDesconectadoQuery()));
        }
    }
}
