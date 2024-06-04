using IGift.Application.Requests.Identity.Users;
using IGift.Shared.Wrapper;
using Microsoft.JSInterop;

namespace Client.Infrastructure.Services.Identity.Authentication
{
    public interface IAuthService
    {
        Task<IResult> Register(UserCreateRequest model);

        /// <summary>
        /// Genera un inicio de sesión en el servidor y si es exitoso guarda las credenciales del usuario en el cliente
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>El resultado de la operación</returns>

        Task<IResult> Login(UserLoginRequest loginModel);
        /// <summary>
        /// Removemos del cliente las credenciales obtenidas a través del token y se cierra la sesión del usuario
        /// </summary>
        /// <returns>Resultado de la operacion</returns>

        Task<IResult> Logout();
        /// <summary>
        /// Este método se usa para activar una función en nuestro archivo scripts que se encarga de desloguer al usuario si se mantiene inactivo por un tiempo específico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dotNetObjectReference"></param>
        /// <returns></returns>
        Task Disconnect<T>(DotNetObjectReference<T> dotNetObjectReference) where T : class;

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

    }
}
