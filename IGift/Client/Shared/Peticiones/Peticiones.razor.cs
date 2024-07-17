using Client.Infrastructure.Services.Requests;
using IGift.Application.Responses.Pedidos;
using Microsoft.AspNetCore.Components;

namespace IGift.Client.Shared.Peticiones
{
    public partial class Peticiones
    {
        [Inject] IPeticiones _peticiones { get; set; }
        private List<PeticionesResponse> _lista { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var response = await _peticiones.GetAll();
        }
    }
}
