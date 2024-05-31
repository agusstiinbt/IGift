using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity;
using IGift.Shared;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = (AppConstants.Role.AdministratorRole))]
        public async Task<ActionResult> GetAll()
        {
           // var users = await _userService.GetAllAsync();
            return Ok();
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Result>> Register(UserCreateRequest m)
        {
            return Ok(await _userService.RegisterAsync(m));
        }
      
    }
}
