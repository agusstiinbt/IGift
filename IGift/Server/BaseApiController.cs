using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server
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

    //El proyecto Shared solo contiene clases reutilizables, no hace falta que IMediator este en una clase "Program.cs" dentro de este proyecto. Cada aplicación Server se encargará de hacer su configuracion de IMediator en su clase Program.cs
}
