using Microsoft.AspNetCore.Components;

namespace IGift.Client.Layouts.Chat.ToolBar
{
    public partial class ChatProfileToggle
    {
        [CascadingParameter]
        public string IdUser { get; set; } = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(IdUser))
            {
                var result = await _profileService.GetByIdAsync(IdUser);
            }
        }
    }
}
