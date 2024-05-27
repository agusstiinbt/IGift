using IGift.Application.Requests.Identity;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public interface IAuthService
    {
        Task<IResult> Register(UserCreateRequest model);
        Task<IResult> Login(UserLoginRequest loginModel);
        Task<IResult> Logout();
        /// <summary>
        /// Este método se usa para activar una función en nuestro archivo scripts que se encarga de desloguer al usuario si se mantiene inactivo por un tiempo específico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dotNetObjectReference"></param>
        /// <returns></returns>
        Task Disconnect<T>(DotNetObjectReference<T> dotNetObjectReference) where T : class;
    }
}
