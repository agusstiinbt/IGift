using IGift.Application.Requests.Identity;
using IGift.Shared.Operations.Login;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public interface IAuthService
    {
        Task<IResult> Register(ApplicationUserRequest model);
        Task<IResult> Login(LoginModel loginModel);
        Task<IResult> Logout();
        Task<IResult> Disconnect<T>(DotNetObjectReference<T> dotNetObjectReference) where T : class;
    }
}
