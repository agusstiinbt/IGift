using IGift.Application.CQRS.Identity.Token;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Responses.Identity.Users;
using IGift.Shared.Wrapper;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<UserLoginResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<UserLoginResponse>> RefreshUserToken(TokenRequest tRequest);
    }
}
