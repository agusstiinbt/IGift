using IGift.Application.Features.Pedidos.Command;
using IGift.Application.Features.Pedidos.Query;
using IGift.Server.Controllers.Base;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeticionesController : BaseApiController<PeticionesController>
    {
        [HttpPost("GuardarPedido")]
        public async Task<ActionResult> EnviarPedido()
        {
            var command = new AddPeticionesCommand { IdUser = "e1cd5123-985f-4a50-bbab-34f2ce253e56", Descripcion = "Tarjeta de regalo", Moneda = "USDT", Monto = 2500 };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("GetAll")]
        public async Task<ActionResult> GetAll(GetAllPeticionesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

    }
}
