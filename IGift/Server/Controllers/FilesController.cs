using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Application.Interfaces.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IGift.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private readonly IProfilePicture _profileService;

        public FilesController(IProfilePicture profileService)
        {
            _profileService = profileService;
        }

        [HttpPost("GetProfilePictureById")]//TODO estudiar el resposnecache que tiene este metodo en el blazor hero
        public async Task<ActionResult> GetProfilePictureAsync(ProfilePictureById p)
        {
            return Ok(await _profileService.GetByUserIdAsync(p.Id));
        }

        [HttpPost("UploadProfilePicture")]
        public async Task<ActionResult> UploadProfileAsync(ProfilePictureUpload request)
        {
            return Ok(await _profileService.SaveProfilePictureAsync(request));
        }
    }
}
