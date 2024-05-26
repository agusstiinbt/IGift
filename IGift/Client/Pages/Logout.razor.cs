using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages
{
    public partial class Logout
    {
        [Inject] IAuthService AuthService { get; set; }
        [Inject] NavigationManager NavigationManager {  get; set; }

        protected override async Task OnInitializedAsync()
        {
            await AuthService.Logout();
            NavigationManager.NavigateTo(AppConstants.Routes.Login);
        }
    }
}
