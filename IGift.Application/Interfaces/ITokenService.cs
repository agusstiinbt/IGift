using IGift.Application.Responses;
using IGift.Shared.Wrapper;
using IGift.Shared.Operations.Login;
namespace IGift.Application.Interfaces
{
    public interface ITokenService
    {
        Task<Result<TokenResponse>> LoginAsync(LoginModel model);
        Task<Result<TokenResponse>> GetRefreshToken(LoginModel model);
    }
}
