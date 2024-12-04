using IGift.Application.CQRS.Files.ProfilePicture;
using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Files
{
    public interface IProfilePicture
    {
        Task<IResult<ProfilePictureResponse>> GetByUserIdAsync(string IdUser);
        Task<IResult> SaveProfilePictureAsync(ProfilePictureUpload request);
    }
}
