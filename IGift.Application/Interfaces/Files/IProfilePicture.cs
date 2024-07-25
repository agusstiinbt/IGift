using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;

namespace IGift.Application.Interfaces.Files
{
    public interface IProfilePicture
    {
        Task<Result<ProfilePictureResponse>> GetByUserIdAsync(string IdUser);
    }
}
