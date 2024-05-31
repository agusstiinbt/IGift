using IGift.Application.Interfaces.Identity;
using IGift.Application.Requests.Token;
using IGift.Application.Requests.Users;
using IGift.Application.Responses.Token;
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
        public async Task<ActionResult<Result<TokenResponse>>> Login(UserLoginRequest m)
        {
            return Ok(await _tokenService.LoginAsync(m));
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Result>> RefreshToken(TokenRequest t)
        {
            var response = await _tokenService.GetRefreshTokenAsync(t);
            return Ok(response);
        }
    }
}
