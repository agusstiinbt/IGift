using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Client.Extensions;
using IGift.Client.Infrastructure.Services.Communication.Chat;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Pages.Communication.Chat
{
    public partial class Chat
    {
        private bool _open = false;

        private void ToggleDrawer()
        {
            _open = !_open;
        }

        [Inject] private IChatManager _chatService { get; set; }
        [CascadingParameter] private HubConnection? _hubConnection { get; set; }

        private List<ChatHistoryResponse> _messages = new();
        private List<ChatUser> Chats { get; set; } = new List<ChatUser>();


        #region Parámetros del usuario actual
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string CurrentUserImageUrl { get; set; }
        #endregion

        #region Parámetros del usuario a chatear

        //TODO posiblemente no todos estos parametros deberían de tener el [parameter]
        [Parameter] public string ChatFullName { get; set; }
        [Parameter] public string ChatId { get; set; }
        [Parameter] public string ChatUserName { get; set; }
        [Parameter] public string ChatImageUrl { get; set; }



        private AuthenticationState? _authenticationState { get; set; } = null;
        private string CurrentUserId { get; set; } = string.Empty;
        private string EstiloBotones = "color:#848E9C";
        #endregion

        protected override async Task OnInitializedAsync()
        {
            _authenticationState = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
            CurrentUserId = _authenticationState.User.GetUserId();

            await LoadChatUsers();
            //await InitializeHub();

        }


        /// <summary>
        /// Inicializamos todas las conexiones de tipo Hub
        /// </summary>
        /// <returns></returns>
        //private async Task InitializeHub()
        //{
        //    //TODO finalizar; Esto se encarga ( entre otras subscripciones más) de dejar en online/offline a los usuarios recibidos por parámetro
        //    _hubConnection.On<string>(AppConstants.SignalR.ConnectUser, (userId) =>
        //    {
        //        // var connectedUser= userli
        //    });


        //    _hubConnection.On<ChatHistory<IChatUser>, string>(AppConstants.SignalR.ReceiveMessage, async (chatHistory, userName) =>
        //    {
        //        if ((ChatId == chatHistory.ToUserId && CurrentUserId == chatHistory.FromUserId) || (ChatId == chatHistory.FromUserId && CurrentUserId == chatHistory.ToUserId))
        //        {
        //            if (ChatId == chatHistory.ToUserId && CurrentUserId == chatHistory.FromUserId)
        //            {
        //                _messages.Add(new ChatHistoryResponse { Message = chatHistory.Message, CreatedDate = chatHistory.CreatedDate, FromUserImageURL = CurrentUserImageUrl });
        //                await _hubConnection.SendAsync(AppConstants.SignalR.SendChatNotification, "New Message From " + userName, ChatId, CurrentUserId);
        //            }
        //            else if (ChatId == chatHistory.FromUserId && CurrentUserId == chatHistory.ToUserId)
        //            {
        //                _messages.Add(new ChatHistoryResponse { Message = chatHistory.Message, CreatedDate = chatHistory.CreatedDate, FromUserImageURL = ChatImageUrl });
        //            }
        //            //TODO finalizar await _jsRuntime.InvokeAsync<string>("ScrollToBottom", "chatContainer");
        //            StateHasChanged();
        //        };
        //    });
        //    //await GetUsersAsync(); Esto se usaría para traer todos los chats que tenemos 

        //    var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
        //    CurrentUserId = state.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

        //    CurrentUserImageUrl = await _localStorage.GetItemAsync<string>(AppConstants.Local.UserImageURL);

        //    if (!string.IsNullOrEmpty(ChatId))
        //    {
        //        await LoadUserChat(ChatId);
        //    }

        //    await _hubConnection.SendAsync(AppConstants.SignalR.PingRequest, CurrentUserId);
        //}

        private async Task LoadChatUsers()
        {
            var result = await _chatManager.LoadChatUsers(new LoadChatUsers() { IdCurrentUser = CurrentUserId });
            if (result.Succeeded)
            {
                Chats = result.Data.ToList();
            }
            else
            {
                _snack.Add(result.Messages.First());
            }
        }

        private async Task OnKeyPressInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                //await SubmitAsync();
            }
        }

        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ChatId))
            {
                //Save Message to DB
                var chatHistory = new SaveChatMessage
                {
                    Message = CurrentMessage,
                    FromUserId = CurrentUserId,
                    ToUserId = ChatId
                };

                var response = await _chatService.SaveMessage(chatHistory);
                if (response.Succeeded)
                {
                    var userName = _authenticationState!.User.GetFirstName();

                    await _hubConnection!.SendAsync(AppConstants.SignalR.SendMessage, chatHistory, userName);
                    CurrentMessage = string.Empty;
                }
                else
                {
                    for (int i = 0; i < response.Messages.Count; i++)
                        _snack.Add(response.Messages[i], Severity.Error);
                }
            }
        }
    }
}