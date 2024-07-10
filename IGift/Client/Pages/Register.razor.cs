using Client.Infrastructure.Services.Identity.Authentication;
using Client.Infrastructure.Services.Interceptor;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Register
    {
        [Inject] private IAuthService _authService { get; set; }
        private UserCreateRequest RegisterModel = new UserCreateRequest();
        [Inject] private IHttpInterceptorManager _interceptor { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _interceptor.RegisterEvent();

        }
        private async Task HandleRegistration()
        {
            var result = await _authService.Register(RegisterModel);

            if (result.Succeeded)
            {
                _nav.NavigateTo(AppConstants.Routes.Login + "/true");
            }
            else
            {
                _snack.Add(result.Messages.First());
            }
        }
    }
}
