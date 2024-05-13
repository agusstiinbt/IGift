using IGift.Shared.Operations.Login;
using IGift.Shared.Operations.Register;
using IGift.Shared.Wrapper;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public interface IAuthService
    {
        Task<RegisterResult> Register(RegisterModel model);
        Task<IResult> Login(LoginModel loginModel);
        Task Logout();
    }
}
