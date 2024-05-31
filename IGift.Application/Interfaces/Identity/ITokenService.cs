using IGift.Shared.Wrapper;
using IGift.Application.Responses.Token;
using IGift.Application.Requests.Token;
using IGift.Application.Requests.Users;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<TokenResponse>> GetRefreshTokenAsync(TokenRequest tRequest);
    }
}
