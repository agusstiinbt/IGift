using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterModel = IGift.Shared.Operations.Register.RegisterModel;
using IGift.Shared.Operations.Register;
using IGift.Infrastructure.Models;
using IGift.Application.Interfaces.Identity;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountsController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("CreateAccount")]
        public async Task<ActionResult> CreateAccount([FromBody] RegisterModel model)
        {

            //var result = await _userService.RegisterAsync(newUser, model.Password!);

            //if (result.Succeeded)
            //{
            //    var errors = result.Errors.Select(x => x.Description);
            //    return Ok(new RegisterResult { Successful = false, Errors = errors });
            //}
            //return Ok(new RegisterResult { Successful = true });
            return Ok();
        }
    }
}
