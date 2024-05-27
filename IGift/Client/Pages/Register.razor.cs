using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Register
    {
        [Inject] private IAuthService _authService { get; set; }
        [Inject] private NavigationManager _nav { get; set; }
        [Inject] private ISnackbar _snackBar { get; set; }
        private UserCreateRequest RegisterModel = new UserCreateRequest();

        private async Task HandleRegistration()
        {
            var result = await _authService.Register(RegisterModel);

            if (result.Succeeded)
            {
                _nav.NavigateTo("/");
            }
            else
            {
                _snackBar.Add(result.Messages.First());
            }
        }
    }
}
