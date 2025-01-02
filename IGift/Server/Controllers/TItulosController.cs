using IGift.Application.CQRS.Titulos.Categoria.Query;
using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TItulosController : BaseApiController<TItulosController>
    {
        [HttpPost]
        public async Task<ActionResult> GetAll(GetAllCategoriaQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
