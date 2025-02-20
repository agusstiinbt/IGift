using Client.Infrastructure.Services.Notification;
using IGift.Application.Responses.Notification;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class Notificaciones
    {
        [Inject] INotificationService _notificationService { get; set; }
        [CascadingParameter] private HubConnection _hubConnection { get; set; }
        [CascadingParameter] public required Task<AuthenticationState> AuthenticationState { get; set; }


        private List<NotificationResponse> list { get; set; } = new();

        private int _notifications { get; set; } = 0;
        private string CurrentUserId { get; set; } = string.Empty;

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
            _visible = _notifications > 0;


        }
        private async Task InitializeHub()
        {
            //TODO implementar una funcionalidad de signalR para recibir notificaciones de todo tipo, correo electronico o chat tambien. En el archivo index.razor.cs tenemos el codigo que levanta un snackbar cuando se recibe una notificacion de tipo chat y ese snackbar tiene una funcionalidad de tipo click que nos re dirigiria a el chat en cuestion. Pero ese snackbar se desaparece en 5 segundos entonces hay que hacer una notificacion en la campanita que haga lo mismo con la funcionalidad de click pero que en este caso no desapareceria
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
