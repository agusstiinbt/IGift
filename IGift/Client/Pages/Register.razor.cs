using Client.Infrastructure.Services.Authentication;
using IGift.Shared.Operations.Register;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages
{
    public partial class Register
    {
        [Inject] private IAuthService _authService { get; set; }
        [Inject] private NavigationManager _nav { get; set; }
        private RegisterModel RegisterModel = new RegisterModel();
        private bool ShowErrors;
        private IEnumerable<string>? Errors;

        private async Task HandleRegistration()
        {
            ShowErrors = false;

            var result = await _authService.Register(RegisterModel);

            if (result.Successful)
            {
                _nav.NavigateTo("/login");
            }
            else
            {
                Errors = result.Errors;
                ShowErrors = true;
            }
        }
    }
}
