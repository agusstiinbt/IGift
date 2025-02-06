using IGift.Application.CQRS.Peticiones.Command;
using IGift.Application.CQRS.Peticiones.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PeticionesController : BaseApiController<PeticionesController>
    {
        [HttpPost("GuardarPedido")]
        public async Task<ActionResult> EnviarPedido()
        {
            var command = new AddEditPeticionesCommand { IdUser = "11f3510c-e716-4c36-b16c-933e7918b06e", Descripcion = "Tarjeta de regalo", Moneda = "USDT", Monto = 2500 };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll(GetAllPeticionesQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
