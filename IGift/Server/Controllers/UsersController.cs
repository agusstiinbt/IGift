using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IGift.Application.Responses;
using IGift.Shared.Operations.Login;
using ITokenService = IGift.Application.Interfaces.Identity.ITokenService;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public UsersController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Result<TokenResponse>>> Login(LoginModel m)
        {
            throw new Exception();
        //    return Ok(await _tokenService.LoginAsync(m));
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Result>> Register(ApplicationUserRequest m)
        {
            return Ok(_userService.RegisterAsync(m));
        }
    }
}
