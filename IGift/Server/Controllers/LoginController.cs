﻿using Microsoft.AspNetCore.Mvc;
using IGift.Shared.Wrapper;
using IGift.Application.Responses;
using IGift.Shared.Operations.Login;
using ITokenService = IGift.Application.Interfaces.ITokenService;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public LoginController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Result<TokenResponse>>> Login(LoginModel m)
        {
            return Ok(await _tokenService.LoginAsync(m));
        }
    }
}
