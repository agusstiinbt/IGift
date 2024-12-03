using System.Net.Http.Headers;
using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toolbelt.Blazor;

namespace Client.Infrastructure.Services.Interceptor
{
    public class HttpInterceptorManager : IHttpInterceptorManager
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly IAuthService _authService;
        private readonly ISnackbar _snackBar;
        private readonly NavigationManager _navigationManager;

        public HttpInterceptorManager(HttpClientInterceptor interceptor, IAuthService authService, ISnackbar snanckBar, NavigationManager navigationManager)
        {
            _interceptor = interceptor;
            _authService = authService;
            _snackBar = snanckBar;
            _navigationManager = navigationManager;
        }

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri.AbsolutePath;
            if (!absPath.ToLower().Contains("token") /*&& !absPath.Contains("accounts")*/)
            {
                try
                {
                    var token = await _authService.TryRefreshToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _snackBar.Add("TokenController refrescado", Severity.Success);
                        e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add("Sesión terminada", Severity.Error);
                    await _authService.Logout();
                    _navigationManager.NavigateTo(AppConstants.Routes.Home);
                }
            }
        }

        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
    }
}
