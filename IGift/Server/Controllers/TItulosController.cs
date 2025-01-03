using IGift.Application.CQRS.Titulos.Categoria.Query;
using IGift.Application.CQRS.Titulos.Titulos.Conectado.Query;
using IGift.Application.CQRS.Titulos.Titulos.Desconectado.Query;
using IGift.Application.Responses.Titulos;
using IGift.Shared.Wrapper;
using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TItulosController : BaseApiController<TItulosController>
    {
        [Obsolete]
        [HttpPost]
        public async Task<ActionResult> GetAll(GetAllCategoriaQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [Obsolete]
        [HttpPost("GetAllTitulosConectado")]
        public async Task<ActionResult> GetAll(GetAllTitulosConectadoQuery query) => Ok(await _mediator.Send(query));

        [Obsolete]
        [HttpPost("GetAllTitulosDesconectado")]
        public async Task<ActionResult> GetAll(GetAllTitulosDesconectadoQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


        [HttpGet("GetBarraHerramientasDesconectado")]
        public async Task<ActionResult> GetAll()
        {
            var response = new BarraHerramientasDesconectadoResponse
            {
                Titulos = (await _mediator.Send(new GetAllTitulosDesconectadoQuery())).Data.ToList(),
                Categorias = (await _mediator.Send(new GetAllCategoriaQuery())).Data.ToList()
            };

            return Ok(await Result<BarraHerramientasDesconectadoResponse>.SuccessAsync(response));
        }
    }
}
