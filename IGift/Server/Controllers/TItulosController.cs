using IGift.Application.CQRS.Titulos.Categoria.Query;
using IGift.Application.CQRS.Titulos.Titulos.Conectado.Query;
using IGift.Application.CQRS.Titulos.Titulos.Desconectado.Query;
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

        [HttpPost("GetAllTitulosConectado")]
        public async Task<ActionResult> GetAll(GetAllTitulosConectadoQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpPost("GetAllTitulosDesconectado")]
        public async Task<ActionResult> GetAll(GetAllTitulosDesconectadoQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
