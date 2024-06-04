using IGift.Shared.Wrapper;
using IGift.Application.Requests.Identity.Token;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Responses;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<LoginResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<LoginResponse>> RefreshUserToken(TokenRequest tRequest);
    }
}
