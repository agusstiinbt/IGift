using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages.Users.Login.Componentes
{
    public partial class HandleLogin
    {
        [Inject] private IAuthService? AuthService { get; set; }

        [Parameter]
        public UserLoginRequest loginModel { get; set; }

        private async Task HandleLoginForm()
        {
            var result = await AuthService.Login(loginModel);

            if (result.Succeeded)
            {
                _nav.NavigateTo(AppConstants.Routes.Home);
            }
            else
            {
                _snack.Add(result.Messages.FirstOrDefault(), Severity.Error);
            }
        }
    }
}
