﻿using IGift.Application.CQRS.Titulos.Conectado;
using IGift.Application.CQRS.Titulos.Desconectado;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TitulosController : BaseApiController<TitulosController>
    {
        [HttpGet("GetBarraHerramientasConectado")]
        public async Task<ActionResult> GetAllConectado()
        {
            return Ok(await _mediator.Send(new GetBarraConectadoQuery()));
        }

        [AllowAnonymous]
        [HttpGet("GetBarraHerramientasDesconectado")]
        public async Task<ActionResult> GetAllDesconectado()
        {
            return Ok(await _mediator.Send(new GetBarraDesconectadoQuery()));
        }
    }
}
