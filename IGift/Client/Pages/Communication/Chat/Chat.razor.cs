using System.Security.Claims;
using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace IGift.Client.Pages.Communication.Chat
{
    public partial class Chat
    {
        [CascadingParameter] private HubConnection? _hubConnection { get; set; }

        /// <summary>
        /// Todos los mensajes de un chat especifico
        /// </summary>
        private List<ChatHistoryResponse> CurrentChat { get; set; } = null;
        private List<ChatUserResponse> Chats { get; set; } = null;

        /// <summary>
        /// El Key, es el UserId. El Value tiene que ser la imagen en base 64
        /// </summary>
        private Dictionary<string, string?>? FotosDeUsuarios { get; set; } = null;

        //private List<ChatHistoryResponse> _messages = new(); Creo que este se deberia de borrar
        private AuthenticationState? _authenticationState { get; set; } = null;

        private Queue<SaveChatMessage> _colaMensajes { get; set; }

        private DotNetObjectReference<Chat>? _dotNetRef;

        /// <summary>
        /// El Id del usuario al cual vamos a enviar o recibir mensajes
        /// </summary>
        public string ToUserId { get; set; }
        private string? CurrentMessage { get; set; }
        private string CurrentUserId { get; set; } = string.Empty;

        private bool _open = true;

        private string IdUser { get; set; } = string.Empty;
        private string Nombre { get; set; }
        private string Apellido { get; set; }
        private string Iniciales { get; set; }

        private string backgroundProfilePicture = string.Empty;

        private bool First { get; set; } = true;

        //Metodos
        protected override async Task OnInitializedAsync()
        {
            _colaMensajes = new Queue<SaveChatMessage>();

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


        [JSInvokable]
        public async Task SendMessageFromJs(string message)
        {
            CurrentMessage = message;
            await SubmitAsync(); // tu lógica de enviar mensaje
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
        private async Task GetThisChatMessages(string ToUserId)
        {
            //TODO Es conveniente siempre que se hace click en el chat buble que se cargue desde el servidor? No seria mejor directamente guardarlo en memoria y capaz que si llego una notificacion de algun chat mediante signalR entonces ahi si traer los ultimos mensajes. De hecho podemos traernos los ultimos mensajes que esten desde el ultimo mensaje ya leido de esa manera se reduce la carga de datos.

            this.ToUserId = ToUserId;
            var response = await _chatManager.GetChatById(new SearchChatById(ToUserId!, CurrentUserId));

            if (response.Succeeded)
            {
                CurrentChat = response.Data.ToList();

                //Lo que esta aca con respecto al statehaschanged lo hacemos porque el inputtext para enviar un mensje se encuentra dentro de una clausula if y hasta que no se renderice entonces no se podra encontrar el input con el id InputChat(algo asi) entoncse no funcionara el codigo js
                StateHasChanged();
                if (First)
                {
                    _dotNetRef = DotNetObjectReference.Create(this);
                    await _JS.InvokeVoidAsync("chatInterop.initializeEnterToSend", _dotNetRef);
                    First = false;
                }
                await ScrollToBottom();

            }
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
                var guardarMensaje = new SaveChatMessage
                {
                    Message = CurrentMessage,
                    FromUserId = CurrentUserId,
                    ToUserId = ToUserId
                };

                CurrentMessage = string.Empty;//Lo dejamos aca para limpiar el front
                _colaMensajes.Enqueue(guardarMensaje);

                AgregarMensajeAlChat(guardarMensaje.Message);

                await ProcesarColaAsync();
            }
        }

        private void AgregarMensajeAlChat(string msg)
        {
            CurrentChat.Add(new ChatHistoryResponse()
            {
                FromUserId = CurrentUserId,
                ToUserId = ToUserId,
                Message = msg,
                Seen = false,
                Date = DateTime.Now,
                Received = false,
                Send = false,
            });
            StateHasChanged();
        }

        private async Task ProcesarColaAsync()
        {
            while (_colaMensajes.TryDequeue(out var msg))
            {
                var result = await _chatManager.SaveMessageAsync(msg);
                if (result.Succeeded)
                {
                    CurrentChat.Last().Send = true;
                    CurrentChat.Last().Received = true;
                }
                else
                {
                    for (int i = 0; i < result.Messages.Count; i++)
                        _snack.Add(result.Messages[i], Severity.Error);
                }
            }
            await ScrollToBottom();

            StateHasChanged();

        }

        /// <summary>
        /// Lleva el scroll hasta abajo de todo
        /// </summary>
        /// <returns></returns>
        private async Task ScrollToBottom()
        {
            await _JS.InvokeVoidAsync("chatInterop.scrollToBottom");

        }

        private void ToggleDrawer() => _open = !_open;
    }
}