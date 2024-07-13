using Client.Infrastructure.Services.Requests;
using IGift.Application.Responses.Pedidos;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Shared.Pedidos
{
    public partial class GrillaPedidos
    {
        [Inject] IPedidosService _pedidos { get; set; }
        private List<PedidosResponse> _lista { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var response = await _pedidos.GetAll();
        }
    }
}
