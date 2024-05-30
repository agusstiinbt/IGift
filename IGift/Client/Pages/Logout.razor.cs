using Client.Infrastructure.Services.Identity.Authentication;
using Client.Infrastructure.Services.Interceptor;
using IGift.Shared;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages
{
    public partial class Logout
    {
        [Inject] IAuthService AuthService { get; set; }
        [Inject] NavigationManager NavigationManager {  get; set; }
        [Inject] private IHttpInterceptorManager _interceptor { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthService.Logout();
            _interceptor.DisposeEvent();
            NavigationManager.NavigateTo(AppConstants.Routes.Login);
        }
    }
}
