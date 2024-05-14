using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IGift.Application.Responses;
using IGift.Shared.Operations.Login;
using ITokenService = IGift.Application.Interfaces.Identity.ITokenService;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public UsersController(ITokenService tokenService)
        {
            _tokenService = tokenService;
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
            return Ok(await _tokenService.LoginAsync(m));
        }
    }
}
