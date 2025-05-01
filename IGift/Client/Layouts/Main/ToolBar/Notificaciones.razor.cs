using IGift.Application.CQRS.Communication.Chat;
using IGift.Application.Enums;
using IGift.Application.Responses.Notification;
using IGift.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class Notificaciones
    {
        [CascadingParameter] private HubConnection _hubConnection { get; set; }
        [CascadingParameter] public required Task<AuthenticationState> AuthenticationState { get; set; }

        private List<NotificationResponse> list { get; set; } = new();

        private bool _open { get; set; }
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await InitializeHub();
            var result = await _notificationService.GetAll();
            if (result.Succeeded)
            {
                list = result.Data.ToList();
            }
            _visible = list.Count > 0;


        }
        private async Task InitializeHub()
        {
            try
            {
                _hubConnection.On<ChatHistoryResponse>(AppConstants.SignalR.ReceiveChatNotificationAsync, (chatHistory) =>
                {
                    var mensaje = "Has recibido un mensaje de " + chatHistory.UserName;
                    list.Add(new NotificationResponse() { DateTime = chatHistory.Date, Message = mensaje, Type = TypeNotification.Chat });

                    _JS.InvokeAsync<string>("PlayAudio", "notification");

                    _snack.Add(mensaje, Severity.Info, config =>
                    {
                        config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
                        config.VisibleStateDuration = 10000;
                        config.Onclick = snackbar =>
                        {
                            _nav.NavigateTo($"chat/{chatHistory.FromUserId}");
                            return Task.CompletedTask;
                        };
                    });

                    StateHasChanged();
                });

            }
            catch (Exception e)
            {
                _snack.Add("Error con el hubconnection en el notificaciones " + e.Message, Severity.Error);
            }
        }

        private void ToggleOpen() => _open = !_open;
    }
}
