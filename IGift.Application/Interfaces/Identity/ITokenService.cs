using IGift.Shared.Wrapper;
using IGift.Application.Requests.Identity.Token;
using IGift.Application.Requests.Identity.Users;
using IGift.Application.Responses;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<TokenResponse>> GetRefreshTokenAsync(TokenRequest tRequest);
    }
}
