using Client.Infrastructure.Services.Identity.Authentication;
using Client.Infrastructure.Services.Interceptor;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages.Users
{
    public partial class Logout
    {
        [Inject] IAuthService AuthService { get; set; }
        [Inject] private IHttpInterceptorManager _interceptor { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await _shopCartService.ClearCache();
            await AuthService.Logout();
            _interceptor.DisposeEvent();
            _nav.NavigateTo(AppConstants.Routes.Home);
        }
    }
}
