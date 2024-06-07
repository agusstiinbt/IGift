using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace IGift.Client.Pages
{
    public partial class Index
    {
        [Inject] private IUserManager? _userManager { get; set; }
        [Inject] private ISnackbar? _snackBar { get; set; }

        public async void HandleSubmit()
        {
            var result = await _userManager.GetUsersAsync();
            if (!result.Succeeded)
            {
                _snackBar.Add(result.Messages.ToString(), Severity.Error);
            }
            _snackBar!.Add("Consulta exitosa");
        }
    }
}
