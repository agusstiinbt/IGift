using Client.Infrastructure.Services.Notification;
using IGift.Application.Responses.Notification;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class Notificaciones
    {
        [Inject] INotificationService _notificationService { get; set; }

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
