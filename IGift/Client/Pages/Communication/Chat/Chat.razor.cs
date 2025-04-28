using Client.Infrastructure.Authentication;
using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Models.Chat;
using IGift.Client.Extensions;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace IGift.Client.Pages.Communication.Chat
{
    public partial class Chat
    {
        //Parametros
        [CascadingParameter] public required Task<AuthenticationState> AuthenticationState { get; set; }
        [Parameter] public string CId { get; set; }

        //Collections
        /// <summary>
        /// Todos los mensajes de un chat especifico
        /// </summary>
        private List<ChatHistoryResponse> CurrentChat { get; set; } = null;
        private List<ChatUserResponse> Chats { get; set; } = null;
        /// <summary>
        /// El Key, es el UserId. El Value tiene que ser la imagen en base 64
        /// </summary>
        private Dictionary<string, string?>? FotosDeUsuarios { get; set; } = null;
        private Queue<SaveChatMessage>? _colaMensajes { get; set; } = null;

        //Otros
        public HubConnection? _hubConnection { get; set; }
        private AuthenticationState? _authenticationState { get; set; } = null;
        private DotNetObjectReference<Chat>? _dotNetRef { get; set; }

        //Strings
        private string backgroundProfilePicture { get; set; } = string.Empty;
        /// <summary>
        /// El Id del usuario al cual vamos a enviar o recibir mensajes
        /// </summary>
        public string ToUserId { get; set; } = string.Empty;
        private string CurrentMessage { get; set; } = string.Empty;
        private string CurrentUserId { get; set; } = string.Empty;

        //Bools
        private bool FirstRender { get; set; } = true;
        private bool _open { get; set; } = true;
        public bool IsHubConnected { get; set; } = false;
        private bool ChatChanged { get; set; } = true;


        //Life Cycles
        protected override async Task OnInitializedAsync()
        {
            await InitializeHub();

            if (IsHubConnected)
            {

                _colaMensajes = new Queue<SaveChatMessage>();
                CurrentChat = new List<ChatHistoryResponse>();
                FotosDeUsuarios = new Dictionary<string, string>();

                _authenticationState = await ((IGiftAuthenticationStateProvider)_authenticationStateProvider).GetAuthenticationStateAsync();

                CurrentUserId = _authenticationState.User.GetUserId();

                await GetProfilePicture();

                await LoadChatUsers();

                if (!string.IsNullOrEmpty(CId))
                    await SelectChatBubble(CId);
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await ScrollToBottom();
        }

        /// <summary>
        /// Inicializamos todas las conexiones de tipo Hub
        /// </summary>
        /// <returns></returns>
        private async Task InitializeHub()
        {
            //TODO finalizar; Esto se encarga ( entre otras subscripciones más) de dejar en online/offline a los usuarios recibidos por parámetro
            //_hubConnection.On<string>(AppConstants.SignalR.ConnectUser, (userId) =>
            //{
            //    // var connectedUser= userli
            //});

            try
            {
                _hubConnection = await _hubConnection.TryInitialize(_nav, _authService, _localStorage);

                if (_hubConnection != null)
                {
                    _hubConnection.On<ChatHistoryResponse>(AppConstants.SignalR.ReceiveMessageAsync, (chatHistory) =>
                     {
                         if (CurrentUserId == chatHistory.ToUserId && ToUserId == chatHistory.FromUserId)
                         {
                             _snack.Add("Has recibido un mensaje de otro usuario1");

                             CurrentChat.Add(chatHistory);

                             //await _hubConnection.SendAsync(AppConstants.SignalR.SendChatNotificationAsync, "Nuevo mensaje de " + userName, ToUserId, CurrentUserId);
                         }
                         StateHasChanged();
                     });


                    IsHubConnected = true;
                }
            }
            catch (Exception e)
            {
                IsHubConnected = false;
                if (e.Message.Contains("401"))
                    _snack.Add("Conexion con SignalR perdida. Proceda con cuidado", Severity.Info, config =>
                    {
                        config.VisibleStateDuration = 3000;
                        config.HideTransitionDuration = 500;
                        config.ShowTransitionDuration = 500;
                        config.ActionColor = Color.Primary;
                    });
                else
                    _snack.Add(e.Message);
            }

            //await _hubConnection.SendAsync(AppConstants.SignalR.PingRequest, CurrentUserId);
        }


        /// <summary>
        /// Traemos del servidor NUESTRA foto de perfil
        /// </summary>
        /// <returns></returns>
        private async Task GetProfilePicture()
        {
            var result = await _profileService.GetByIdAsync(CurrentUserId!);

            if (result.Succeeded)
            {
                var imageBase64 = Convert.ToBase64String(result.Data.Data);

                backgroundProfilePicture = $"width: 80px; height: 60px; background-image: url('data:image/jpg;base64,{imageBase64}'); background-size: cover;";

                FotosDeUsuarios!.Add(CurrentUserId, imageBase64);
            }
            else
            {
                _snack.Add(result.Messages.First(), Severity.Error);
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
        private async Task SelectChatBubble(string ToUserId)
        {
            if (!CurrentChat.Any(x => x.ToUserId == ToUserId))
            {
                this.ToUserId = ToUserId;
                var response = await _chatManager.GetChatById(new SearchChatById(ToUserId!, CurrentUserId));

                if (response.Succeeded)
                {
                    CurrentChat = response.Data.ToList();

                    //Lo que esta aca con respecto al statehaschanged lo hacemos porque el inputtext para enviar un mensje se encuentra dentro de una clausula if y hasta que no se renderice entonces no se podra encontrar el input con el id InputChat(algo asi) entoncse no funcionara el codigo js
                    StateHasChanged();
                    if (FirstRender)
                    {
                        _dotNetRef = DotNetObjectReference.Create(this);
                        await _JS.InvokeVoidAsync("chatInterop.initializeEnterToSend", _dotNetRef);
                        FirstRender = false;
                    }
                    StateHasChanged();
                    ChatChanged = true;
                }
                else
                    _snack.Add(response.Messages.First());
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
                var guardarMensaje = new SaveChatMessage
                {
                    Message = CurrentMessage,
                    FromUserId = CurrentUserId,
                    ToUserId = ToUserId
                };

                var chatHistory = new ChatHistoryResponse()
                {
                    FromUserId = CurrentUserId,
                    ToUserId = ToUserId,
                    Message = CurrentMessage,
                    Date = DateTime.Now,
                    Seen = false,
                    Received = false,
                    Send = false,
                };

                //Agregamos el mensaje a la cola
                _colaMensajes!.Enqueue(guardarMensaje);
                CurrentMessage = string.Empty;
                //Procesamos la cola
                while (_colaMensajes.TryDequeue(out var msg))
                {
                    CurrentChat.Add(chatHistory);

                    var result = await _chatManager.SaveMessageAsync(msg);
                    if (result.Succeeded)
                    {
                        CurrentChat.Last().Send = true;
                        CurrentChat.Last().Received = true;
                        StateHasChanged();
                        await _hubConnection.SendAsync(AppConstants.SignalR.SendChatNotificationAsync, chatHistory);
                    }
                    else
                    {
                        for (int i = 0; i < result.Messages.Count; i++)
                            _snack.Add(result.Messages[i], Severity.Error);
                    }
                }
            }
        }


        [JSInvokable]
        public async Task SendMessageFromJs(string message)
        {
            CurrentMessage = message;
            await SubmitAsync();
        }


        /// <summary>
        /// Lleva el scroll hasta abajo de todo
        /// </summary>
        /// <returns></returns>
        private async Task ScrollToBottom() => await _JS.InvokeVoidAsync("chatInterop.scrollToBottom");


        private void ToggleDrawer() => _open = !_open;
    }
}