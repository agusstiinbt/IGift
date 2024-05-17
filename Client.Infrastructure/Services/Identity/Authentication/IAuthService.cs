using IGift.Application.Requests.Identity;
using IGift.Shared.Operations.Login;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public interface IAuthService
    {
        Task<IResult> Register(ApplicationUserRequest model);
        Task<IResult> Login(LoginModel loginModel);
        Task Logout();
    }
}
