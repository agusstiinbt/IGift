using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Client.Infrastructure.Services.Interceptor;

namespace IGift.Client.Shared
{
    public partial class NavMenu
    {

        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        [Inject] private IHttpInterceptorManager _interceptor { get; set; }


        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
        protected override async Task OnParametersSetAsync()
        {
            //TODO revisar para qué nos puede servir esto en el nav como tienen en BlazorHero. Esto lleva tiempo
            // var claimsPrincipal = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateProviderUserAsync();
            _interceptor.RegisterEvent();
        }
    }
}
