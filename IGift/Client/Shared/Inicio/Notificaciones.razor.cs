using Client.Infrastructure.Authentication;
using Client.Infrastructure.Services.Notification;
using Client.Infrastructure.Services.Identity.Authentication;
using IGift.Application.Requests.Identity.Users;
using IGift.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using IGift.Application.Responses.Chat;

namespace IGift.Client.Shared.Inicio
{
    public partial class Notificaciones
    {
        [Inject] INotificationService _notificationService { get; set; }

        private List<NotificationResponse> list { get; set; } = new();

        private string _buttonText = "Reply";
        private int _notifications { get; set; }
        public bool _open;
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var result = await _notificationService.GetAll();
            _notifications = result.Count;
            _visible = _notifications == 0 ? false : true;
        }

        private void ToggleOpen()
        {
            if (_open)
                _open = false;
            else
                _open = true;
        }

        private void SetButtonText(int id)
        {
            switch (id)
            {
                case 0:
                    _buttonText = "Reply";
                    break;
                case 1:
                    _buttonText = "Reply All";
                    break;
                case 2:
                    _buttonText = "Forward";
                    break;
                case 3:
                    _buttonText = "Reply & Delete";
                    break;
            }
        }
    }
}
