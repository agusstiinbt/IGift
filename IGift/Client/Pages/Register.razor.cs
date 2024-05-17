using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages
{
    public partial class Register
    {
        [Inject] private IAuthService _authService { get; set; }
        [Inject] private NavigationManager _nav { get; set; }
        private ApplicationUserRequest RegisterModel = new ApplicationUserRequest();
        private bool ShowErrors;
        private IEnumerable<string>? Errors;

        private async Task HandleRegistration()
        {
            ShowErrors = false;

            var result = await _authService.Register(RegisterModel);

            if (result.Succeeded)
            {
                _nav.NavigateTo("/login");
            }
            else
            {
                Errors = result.Messages;
                ShowErrors = true;
            }
        }
    }
}
