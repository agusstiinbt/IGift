﻿using IGift.Application.CQRS.Categoria.Query;
using IGIFT.Server.Shared;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : BaseApiController<CategoriaController>
    {
        [HttpPost]
        public async Task<ActionResult> GetAll(GetAllCategoriaQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
