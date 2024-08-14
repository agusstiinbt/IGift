using Client.Infrastructure.Services.Notification;
using IGift.Application.Requests.LocalesAdheridos.Command;
using IGift.Application.Requests.Peticiones.Command;
using IGift.Application.Responses.Notification;
using IGift.Client.Infrastructure.Services.CarritoDeCompras;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Layouts.Main.ToolBar
{
    public partial class CarritoCompras
    {

        [Inject] ICarritoComprasService _carritoCompras { get; set; }

        private List<AddEditPeticionesCommand> list { get; set; } = new();

        private int _peticiones { get; set; } = 0;
        public bool _open;
        private bool _visible { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var result = await _carritoCompras.ObtenerCarritoDePeticiones();
            if (result.Succeeded)
            {
                list = result.Data;
                _peticiones = list.Count;
            }
            _visible = _peticiones == 0 ? false : true;
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
