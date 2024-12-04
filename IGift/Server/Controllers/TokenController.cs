using IGift.Application.CQRS.Identity.Token;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Interfaces.Identity;
using IGift.Application.Responses.Identity.Users;
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
        public async Task<ActionResult<Result<UserLoginResponse>>> Login(UserLoginRequest m)
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
