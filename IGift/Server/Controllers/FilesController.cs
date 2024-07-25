using IGift.Application.Requests.Files;
using IGift.Server.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : BaseApiController<FilesController>
    {
        [HttpPost]
        public async Task<ActionResult> GetProfilePictureAsync(GetProfilePictureQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
