using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared;
using IGift.Shared.Operations.Login;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages
{
    public partial class Login
    {
        [Inject] private IAuthService AuthService { get; set; }

        private LoginModel loginModel = new LoginModel();
        private bool ShowErrors;
        private string Error = "";

        private async Task HandleLogin()
        {
            ShowErrors = false;

            var result = await AuthService.Login(loginModel);

            if (result.Succeeded)
            {
                NavigationManager.NavigateTo(AppConstants.Routes.Home);
            }
            else
            {
                Error = result.Messages.First();
                ShowErrors = true;
            }
        }
    }
}
