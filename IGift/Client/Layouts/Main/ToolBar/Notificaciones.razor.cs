using Client.Infrastructure.Services.Notification;
using IGift.Application.Responses.Notification;
using IGift.Client.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class Notificaciones
    {
        [Inject] INotificationService _notificationService { get; set; }

        [CascadingParameter] private HubConnection _hubConnection { get; set; }

        private List<NotificationResponse> list { get; set; } = new();

        private int _notifications { get; set; } = 0;
        public bool _open;
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var result = await _notificationService.GetAll();
            if (result.Succeeded)
            {
                list = result.Data.ToList();
                _notifications = list.Count;
            }
            _visible = _notifications == 0 ? false : true;


            _hubConnection = await _hubConnection.TryInitialize(_nav, _localStorage);

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        private void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }
    }

}
