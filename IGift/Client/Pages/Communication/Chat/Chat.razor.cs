using IGift.Application.Models.Chat;
using IGift.Application.Requests.Identity.Users;
using IGift.Client.Extensions;
using IGift.Client.Infrastructure.Services.Communication.Chat;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Claims;
using System.Security.Cryptography;

namespace IGift.Client.Pages.Communication.Chat
{
    public partial class Chat
    {
        [Inject] private IChatService _chatService { get; set; }
        [CascadingParameter] private HubConnection _hubConnection { get; set; }


        /// <summary>
        /// Mensajes de un chat
        /// </summary>
        private List<ChatHistory> _messages = new();


        #region Parámetros del usuario actual
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string CurrentUserId { get; set; }
        [Parameter] public string CurrentUserImageUrl { get; set; }
        #endregion

        #region Parámetros del usuario a chatear

        [Parameter] public string ChatFullName { get; set; }
        [Parameter] public string ChatId { get; set; }
        [Parameter] public string ChatUserName { get; set; }
        [Parameter] public string ChatImageUrl { get; set; }

        #endregion


        protected override async Task OnInitializedAsync()
        {
            _hubConnection = _hubConnection.TryInitialize(_nav, _localStorage);
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }

            _hubConnection.On<ChatHistory, string>(AppConstants.SignalR.ReceiveMessage, async (chatHistory, userName) =>
            {
                if ((ChatId == chatHistory.ToUserId && CurrentUserId == chatHistory.FromUserId) || (ChatId == chatHistory.FromUserId && CurrentUserId == chatHistory.ToUserId))
                {
                    if (ChatId == chatHistory.ToUserId && CurrentUserId == chatHistory.FromUserId)
                    {
                        _messages.Add(new ChatHistory { Message = chatHistory.Message, Date = chatHistory.Date, FromUserImageUrl = CurrentUserImageUrl });
                        await _hubConnection.SendAsync(AppConstants.SignalR.SendChatNotification, "New Message From " + userName, ChatId, CurrentUserId);
                    }
                    else if (ChatId == chatHistory.FromUserId && CurrentUserId == chatHistory.ToUserId)
                    {
                        _messages.Add(new ChatHistory { Message = chatHistory.Message, Date = chatHistory.Date, FromUserImageUrl = ChatImageUrl });
                    }
                    //TODO finalizar await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
                    StateHasChanged();
                }
            });
            //await GetUsersAsync();
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            CurrentUserId = state.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

            CurrentUserImageUrl = await _localStorage.GetItemAsync<string>(AppConstants.StorageConstants.Local.UserImageURL);

            if (!string.IsNullOrEmpty(ChatId))
            {
                await LoadUserChat(ChatId);
            }
            await _hubConnection.SendAsync(AppConstants.SignalR.PingRequest, CurrentUserId);
        }

        private async Task LoadUserChat(string userId)
        {
            _open = false;
            var response = await _userManager.GetUserById(userId);
            if (response.Succeeded)
            {
                //var contact = response.Data;
                //ChatId = contact.Id;
                //ChatFullName = contact.FirstName + " " + contact.LastName;
                //ChatUserName = contact.FirstName;
                //ChatImageUrl=contact
            }
        }

    }
}
