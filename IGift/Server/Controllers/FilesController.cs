using IGift.Application.Interfaces.Files;
using IGift.Application.Requests.Files.ProfilePicture;
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
        public async Task<ActionResult> GetProfilePictureAsync(ProfilePictureById p)
        {
            var response = await _profileService.GetByUserIdAsync(p.Id);
            return Ok(response);
        }

        [HttpPost("UploadProfilePicture")]
        public async Task<ActionResult> UploadProfileAsync(ProfilePictureUpload request)
        {
            return Ok();
        }

    }
}
