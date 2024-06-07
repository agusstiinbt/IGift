using IGift.Application.Interfaces.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IUserService _userService;

        public RolesController(IUserService userService)
        {
            _userService = userService;
        }

        //[HttpGet]
        //public async Task<ActionResult> GetAllRoles(string Id)
        //{
        //    return Ok(await _userService.GetRolesAsync(Id));
        //}

    }
}
