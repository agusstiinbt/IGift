using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IGift.Application.Interfaces.Identity;
using IGift.Shared;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Requests.Identity.Password;

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
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("GetById")]
        [Authorize(Roles = (AppConstants.Role.AdministratorRole))]
        public async Task<ActionResult<Result>> GetById(string id)
        {
            return Ok(await _userService.GetByIdAsync(id));
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Result>> Register(UserCreateRequest m)
        {
            return Ok(await _userService.RegisterAsync(m));
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _userService.ForgotPasswordAsync(request, origin));
        }

    }
}
