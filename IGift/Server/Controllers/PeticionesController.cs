using IGift.Application.Requests.Peticiones.Command;
using IGift.Application.Requests.Peticiones.Query;
using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [AllowAnonymous]
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
        public async Task<ActionResult> GetAll(GetAllPeticionesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
