using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace IGift.Client.Pages.Communication.Chat
{
    public partial class Chat
    {
        [CascadingParameter] private HubConnection? _hubConnection { get; set; }

        private List<ChatHistoryResponse> CurrentChat = null;
        private List<ChatUserResponse> Chats { get; set; } = null;

        /// <summary>
        /// El Key, es el UserId. El Value tiene que ser la imagen en base 64
        /// </summary>
        private Dictionary<string, string?>? FotosDeUsuarios { get; set; } = null;

        //private List<ChatHistoryResponse> _messages = new(); Creo que este se deberia de borrar
        private AuthenticationState? _authenticationState { get; set; } = null;


        /// <summary>
        /// El Id del usuario al cual vamos a enviar o recibir mensajes
        /// </summary>
        public string ToUserId { get; set; }
        private string? CurrentMessage { get; set; }
        private string CurrentUserId { get; set; } = string.Empty;

        private bool _open = false;
        private string background { get; set; }

        private string IdUser { get; set; } = string.Empty;
        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }


        private string backgroundProfilePicture = string.Empty;


        //Metodos
        protected override async Task OnInitializedAsync()
        {

            FotosDeUsuarios = new Dictionary<string, string>();

            _authenticationState = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();
            CurrentUserId = _authenticationState.User.GetUserId();

            var user = _authenticationState.User;

            if (_authenticationState != null && user.Identity.IsAuthenticated)
            {
                Nombre = user.FindFirst(c => c.Type == ClaimTypes.Name)?.Value!;
                Apellido = user.FindFirst(c => c.Type == ClaimTypes.Surname)?.Value!;
                IdUser = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            }
            await GetProfilePicture();

            await LoadChatUsers();
            //await InitializeHub();
        }

        ///// <summary>
        ///// Inicializamos todas las conexiones de tipo Hub
        ///// </summary>
        ///// <returns></returns>
        //private async Task InitializeHub()
        //{
        //    //TODO finalizar; Esto se encarga ( entre otras subscripciones más) de dejar en online/offline a los usuarios recibidos por parámetro
        //    //_hubConnection.On<string>(AppConstants.SignalR.ConnectUser, (userId) =>
        //    //{
        //    //    // var connectedUser= userli
        //    //});


        //    _hubConnection.On<SaveChatMessage, string>(AppConstants.SignalR.ReceiveMessageAsync, async (chatHistory, userName) =>
        //    {
        //        if (CurrentUserId == chatHistory.FromUserId)
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
        //        }
        //        ;
        //    }
        //    );
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



        /// <summary>
        /// Cargamos NUESTRA foto de perfil
        /// </summary>
        /// <returns></returns>
        private async Task GetProfilePicture()
        {
            var result = await _profileService.GetByIdAsync(IdUser!);

            if (result.Succeeded)
            {
                var imageBase64 = Convert.ToBase64String(result.Data.Data);

                backgroundProfilePicture = $"width: 80px; height: 60px; background-image: url('data:image/jpg;base64,{imageBase64}'); background-size: cover;";

                FotosDeUsuarios!.Add(CurrentUserId, imageBase64);
            }
            else
            {
                Iniciales = Nombre.Substring(0, 1).ToUpper() + Apellido.Substring(0, 1).ToUpper();
            }
        }


        /// <summary>
        /// Carga la grilla de chats en el lazo izquierdo
        /// </summary>
        /// <returns></returns>
        private async Task LoadChatUsers()
        {
            var result = await _chatManager.LoadChatUsers(new LoadChatUsers() { IdCurrentUser = CurrentUserId });

            if (result.Succeeded)
            {
                var imageBase64 = string.Empty;

                Chats = result.Data.ToList();

                for (int i = 0; i < Chats.Count; i++)
                {
                    imageBase64 = Convert.ToBase64String(Chats[i].Data);
                    FotosDeUsuarios!.Add(Chats[i].UserId, imageBase64);
                }
            }
            else
                _snack.Add(result.Messages.First());
        }


        /// <summary>
        /// Abre el chat seleccionado y pone el ultimo mensaje como visto. 
        /// </summary>
        /// <param name="ToUserId"></param>
        /// <returns></returns>
        private async Task SelectedChatBubble(string ToUserId)
        {
            this.ToUserId = ToUserId;
            var response = await _chatManager.GetChatById(new SearchChatById(ToUserId!, CurrentUserId));

            if (response.Succeeded)
                CurrentChat = response.Data.ToList();
            else
                _snack.Add(response.Messages.First());
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
                var message = new SaveChatMessage
                {
                    Message = CurrentMessage,
                    FromUserId = CurrentUserId,
                    ToUserId = ToUserId
                };

                var response = await _chatManager.SaveMessage(message);

                if (response.Succeeded)
                {
                    var userName = _authenticationState!.User.GetFirstName();

                    CurrentChat.Add(new ChatHistoryResponse()
                    {
                        FromUserId = CurrentUserId,
                        ToUserId = ToUserId,
                        Message = CurrentMessage,
                        Seen = false,
                        Date = DateTime.Now,
                    });
                    StateHasChanged();

                    try
                    {
                        //await _hubConnection!.SendAsync(AppConstants.SignalR.SendChatNotificationAsync, saveChatMessage, CurrentUserId);

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