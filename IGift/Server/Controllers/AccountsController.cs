using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegisterModel = IGift.Shared.Operations.Register.RegisterModel;
using IGift.Shared.Operations.Register;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<Infrastructure.Models.IGiftUser> _userManager;

        public AccountsController(UserManager<Infrastructure.Models.IGiftUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] RegisterModel model)
        {
            var newUser = new Infrastructure.Models.IGiftUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(newUser, model.Password!);

            if (result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return Ok(new RegisterResult { Successful = false, Errors = errors });
            }
            return Ok(new RegisterResult { Successful = true });
        }
    }
}
