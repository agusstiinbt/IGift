using IGift.Application.CQRS.Identity.Token;
using IGift.Application.CQRS.Identity.Users;
using IGift.Application.Responses.Identity.Users;
using IGift.Shared.Wrapper;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<TokenResponse>> RefreshUserToken(TokenRequest tRequest);
    }
}
