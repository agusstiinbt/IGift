using IGift.Application.Interfaces.Files;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IProfilePicture _profileService;

        public FilesController(IProfilePicture profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("GetById")]
        public async Task<ActionResult> GetProfilePictureAsync(string IdUser)
        {
            var response = await _profileService.GetByUserIdAsync(IdUser);
            return Ok(response);
        }
    }
}
