using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Shared.Login
{
    public partial class HandleLogin
    {
        [Inject] private IAuthService? AuthService { get; set; }
        [Inject] private ISnackbar? _snackBar { get; set; }
        [Inject] private NavigationManager _navigationManager { get; set; }


        [Parameter]
        public UserLoginRequest loginModel { get; set; }

        private async Task HandleLoginForm()
        {
            var result = await AuthService.Login(loginModel);

            if (result.Succeeded)
            {
                _navigationManager.NavigateTo(AppConstants.Routes.Home);
            }
            else
            {
                _snackBar.Add(result.Messages.FirstOrDefault(), Severity.Error);
            }
        }
    }
}
