using IGift.Shared.Constants;

namespace IGift.Client.Pages.Users
{
    public partial class Logout
    {
        protected override async Task OnInitializedAsync()
        {
            await _shopCartService.ClearCache();
            await _authService.Logout();
            _interceptor.DisposeEvent();
            _nav.NavigateTo(AppConstants.Routes.Home);
        }
    }
}
