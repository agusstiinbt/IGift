using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers.Base
{
    /// <summary>
    /// Clase abstracta
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController<T> : ControllerBase
    {
        private IMediator _mediatorInstance;
        protected IMediator _mediator => _mediatorInstance ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}
