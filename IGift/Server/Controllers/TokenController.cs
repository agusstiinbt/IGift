using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Identity.Token;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Result<LoginResponse>>> Login(UserLoginRequest m)
        {
            return Ok(await _tokenService.LoginAsync(m));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Result>> RefreshToken(TokenRequest t)
        {
            var response = await _tokenService.RefreshUserToken(t);
            return Ok(response);
        }
    }
}
