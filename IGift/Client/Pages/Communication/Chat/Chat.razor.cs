﻿using IGift.Application.Models.Chat;
using IGift.Client.Extensions;
using IGift.Client.Infrastructure.Services.Communication.Chat;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Security.Claims;

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

        //TODO posiblemente no todos estos parametros deberían de tener el [parameter]
        [Parameter] public string ChatFullName { get; set; }
        [Parameter] public string ChatId { get; set; }
        [Parameter] public string ChatUserName { get; set; }
        [Parameter] public string ChatImageUrl { get; set; }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            await InitializeHub();
        }


        /// <summary>
        /// Inicializamos todas las conexiones de tipo Hub
        /// </summary>
        /// <returns></returns>
        private async Task InitializeHub()
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
            //await GetUsersAsync(); Esto se usaría para traer todos los chats que tenemos 

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
                var contact = response.Data;
                ChatId = contact.Id;
                ChatFullName = contact.FirstName + " " + contact.LastName;
                ChatUserName = contact.FirstName;
                ChatImageUrl = contact.Url;
                _nav.NavigateTo(AppConstants.Routes.Chat + "/" + ChatId);
                _messages = new();
                var historyResponse = await _chatService.GetChatHistoryByIdAsync(ChatId);
                if (historyResponse.Succeeded)
                {
                    _messages = historyResponse.Data.ToList();
                }
                else
                {
                    foreach (var message in historyResponse.Messages)
                    {
                        _snack.Add(message, Severity.Error);
                    }
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snack.Add(message, Severity.Error);
                }
            }
        }

        //TODO terminar de implementar

        /// <summary>
        /// Este método se encarga de traernos todos los chats que tenemos pendientes y no han sido aún eliminados
        /// </summary>
        /// <returns></returns>
        private async Task GetUsersAsync() { }

        private async Task OnKeyPressInChat(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SubmitAsync();
            }
        }

        private async Task SubmitAsync()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ChatId))
            {
                //Save Message to DB
                var chatHistory = new ChatHistory
                {
                    Message = CurrentMessage,
                    ToUserId = ChatId,
                    Date = DateTime.Now
                };

                var response = await _chatService.SaveMessage(chatHistory);
                if (response.Succeeded)
                {
                    var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
                    var user = state.User;
                    //CurrentUserId = user.GetUserId();
                    //chatHistory.FromUserId = CurrentUserId;
                    //var userName = $"{user.GetFirstName()} {user.GetLastName()}";
                    //await HubConnection.SendAsync(ApplicationConstants.SignalR.SendMessage, chatHistory, userName);
                    //CurrentMessage = string.Empty;
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        // _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}
