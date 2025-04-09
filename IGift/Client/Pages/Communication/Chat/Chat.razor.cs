using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Client.Extensions;
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
        #region Parametros
        [CascadingParameter] private HubConnection? _hubConnection { get; set; }

        private List<ChatHistoryResponse> CurrentChat = null;
        private List<ChatUserResponse> Chats { get; set; } = new List<ChatUserResponse>();
        //private List<ChatHistoryResponse> _messages = new(); Creo que este se deberia de borrar

        private AuthenticationState? _authenticationState { get; set; } = null;

        /// <summary>
        /// El Id del usuario al cual vamos a enviar o recibir mensajes
        /// </summary>
        public string ToUserId { get; set; }
        private string? CurrentMessage { get; set; }
        private string CurrentUserId { get; set; } = string.Empty;

        private bool _open = false;

        #endregion

        //Metodos
        protected override async Task OnInitializedAsync()
        {
            _authenticationState = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
            CurrentUserId = _authenticationState.User.GetUserId();

            await LoadChatUsers();
            //await InitializeHub();
        }

        /// <summary>
        /// Abre el chat seleccionado y pone el ultimo mensaje como visto. NEcesitamos pasarle el Id del usuario con el que queremos chatear
        /// </summary>
        /// <param name="ToUserId"></param>
        /// <returns></returns>
        private async Task GetChatById(string ToUserId)
        {
            this.ToUserId = ToUserId;
            var response = await _chatManager.GetChatById(new GetChatById(ToUserId));

            if (response.Succeeded)
                CurrentChat = response.Data.ToList();
            else
                _snack.Add(response.Messages.First());
        }

        ///// <summary>
        ///// Inicializamos todas las conexiones de tipo Hub
        ///// </summary>
        ///// <returns></returns>
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
                Chats = result.Data.ToList();
            else
                _snack.Add(result.Messages.First());
        }

        private async Task OnKeyPressInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                //await SubmitAsync();
            }
        }

        /// <summary>
        /// Envia un mensaje al chat correspondiente al usuario conectado y al usuario destino
        /// </summary>
        /// <returns></returns>
        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ToUserId))
            {
                //Save Message to DBs
                var chat = new SaveChatMessage
                {
                    Message = CurrentMessage,
                    FromUserId = CurrentUserId,
                    ToUserId = ToUserId
                };

                var response = await _chatManager.SaveMessage(chat);

                if (response.Succeeded)
                {
                    var userName = _authenticationState!.User.GetFirstName();

                    try
                    {
                        await _hubConnection!.SendAsync(AppConstants.SignalR.SendChatNotificationAsync, chat, CurrentUserId);

                        //await _hubConnection!.SendAsync(AppConstants.SignalR.SendMessageAsync, chat, CurrentUserId, userName);
                    }
                    catch (Exception)
                    {
                        _snack.Add("Ocurrio un problema con SignalR. Proceda con cuidado", Severity.Warning);
                    }
                    CurrentMessage = string.Empty;
                }
                else
                    for (int i = 0; i < response.Messages.Count; i++)
                        _snack.Add(response.Messages[i], Severity.Error);
            }
        }

        private void ToggleDrawer() => _open = !_open;
    }
}