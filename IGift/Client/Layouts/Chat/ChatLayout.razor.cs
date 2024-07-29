using Client.Infrastructure.Authentication;
using System.Security.Claims;

namespace IGift.Client.Layouts.Chat
{
    public partial class ChatLayout
    {
        public string IdUser { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var state = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
            var user = state.User;
            if (state != null && user.Identity.IsAuthenticated)
            {
                IdUser = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            }
        }
    }
}
