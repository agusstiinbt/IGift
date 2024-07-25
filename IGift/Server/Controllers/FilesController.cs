using IGift.Application.Interfaces.Files;
using IGift.Application.Requests.Files;
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

        [HttpPost("GetProfilePictureById")]
        public async Task<ActionResult> GetProfilePictureAsync(ProfilePictureRequest p)
        {
            var response = await _profileService.GetByUserIdAsync(p.Id);
            return Ok(response);
        }
    }
}
