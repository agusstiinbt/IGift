using Microsoft.AspNetCore.Components;

namespace IGift.Client.Pages.Communication.Chat
{
    public partial class Chat
    {
        [Parameter] public string CId { get; set; }

        protected override async Task OnInitializedAsync()
        {

        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    //await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");//TODO implementar
        //}

    }
}
