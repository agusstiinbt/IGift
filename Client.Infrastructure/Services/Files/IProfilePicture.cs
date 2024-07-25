using IGift.Application.Responses.Files;
using IGift.Shared.Wrapper;

namespace IGift.Client.Infrastructure.Services.Files
{
    public interface IProfilePicture
    {
        Task<IResult<ProfilePictureResponse>> GetByIdAsync(string Id);
    }
}
