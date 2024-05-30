using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using IGift.Application.Requests.Identity;
namespace IGift.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> LoginAsync(UserLoginRequest model);
        Task<Result<TokenResponse>> GetRefreshTokenAsync(TokenRequest tRequest);
    }
}
