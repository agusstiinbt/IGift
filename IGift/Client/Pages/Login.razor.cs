using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Shared;
using IGift.Shared.Operations.Login;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Login
    {
        [Inject] private IAuthService AuthService { get; set; }
        [Inject] private ISnackbar _snackBar { get; set; }

        private LoginModel loginModel = new LoginModel();

        private async Task HandleLogin()
        {
            var result = await AuthService.Login(loginModel);

            if (result.Succeeded)
            {
                NavigationManager.NavigateTo(AppConstants.Routes.Home);
            }
            else
            {
                _snackBar.Add(result.Messages.FirstOrDefault(),Severity.Error);
            }
        }
    }
}
