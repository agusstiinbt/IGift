using Client.Infrastructure.Services.Identity.Users;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages
{
    public partial class Index
    {
        [Inject] private IUserManager _userManager { get; set; }

        public async void HandleSubmit()
        {
            var result= await _userManager.GetUsersAsync();
            if (result.Succeeded)
            {

            }
        }
    }
}
