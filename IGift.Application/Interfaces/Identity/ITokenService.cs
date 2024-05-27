using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using IGift.Application.Requests.Identity;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<ApplicationUserResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<ApplicationUserResponse>> GetRefreshToken(UserLoginRequest model);
    }
}
