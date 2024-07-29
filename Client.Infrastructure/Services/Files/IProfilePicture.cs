using IGift.Application.Requests.Files.ProfilePicture;
using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Forms;

namespace IGift.Client.Infrastructure.Services.Files
{
    public interface IProfilePicture
    {
        Task<IResult<ProfilePictureResponse>> GetByIdAsync(string Id);
        Task<IResult> UploadAsync(ProfilePictureUpload Id);
    }
}
